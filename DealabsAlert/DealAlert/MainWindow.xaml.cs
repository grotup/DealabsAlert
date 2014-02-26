using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DealabsAlert;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Windows.Shell;
using System.Windows.Threading;
using System.ComponentModel;
using log4net;

namespace DealAlert
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class DealabsAlert : Window
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DealabsAlert));

        DealabsParser parser;
        DateTime DateDernierItem;
        List<DealabsItem> ListeAffichee;
        DealabsItem ItemSelectionne;

        public DealabsAlert()
        {
            log.Debug("Lancement application");
            InitializeComponent();
            log.Debug("Initialisation configuration");
            parser = new DealabsParser(ConfigurationSettings.AppSettings["url"], Convert.ToInt16(ConfigurationSettings.AppSettings["refreshMinutes"]));
            
            // On fait un premier update
            log.Debug("Premier update Dealabs");
            parser.updateItems();

            log.Debug("Affichage de la liste dans l'interface");
            this.ListeAffichee = parser.GetList(string.Empty);
            this.listBox1.ItemsSource = this.ListeAffichee;
            
            log.Debug("Mise à jour élements interface");
            lnNbItems.Content = listBox1.Items.Count + " élement(s).";
            btnOuvrirUrl.IsEnabled = false;
            Btn_OuvrirDealExterne.IsEnabled = false;
            Tbk_Code.IsReadOnly = true;

            log.Debug("Lancement du worker de mise à jour");
            LancerWorkerMAJ();
            
        }

        private void LancerWorkerMAJ()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 60);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(Object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_UpdateUI;
            worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            DateDernierItem = this.ListeAffichee.ElementAt(0).date;
            log.Debug("Update dealabs en tache de fond");
            parser.updateItems();
        }

        private void worker_UpdateUI(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((parser.DateDernierItem.CompareTo(DateDernierItem) > 0))
            {
                log.Debug("Récupération des nouveaux items");
                List<DealabsItem> liste = parser.getNouveauxDeals(DateDernierItem);
                // Si on a un filtre de défini, on boucle, sinon, on pop juste le nombre d'items nouveaux
                string filtre = ConfigurationSettings.AppSettings["filtre"];
                // On définit le message à comme étant le nombre de nouveaux deals
                log.Debug(liste.Count + " nouveaux deal(s)");
                string message = liste.Count + " nouveaux deal(s) !";
                // Si on a un filtre, on boucle
                if (!string.IsNullOrEmpty(filtre))
                {
                    for (int i = 0; i < liste.Count; i++)
                    {
                        // Si on trouve un item, on définit le message comme étant le titre du deal
                        if (liste.ElementAt(i).titre.Contains(filtre))
                        {
                            log.Debug("Nouvel item correspondant au filtre : " + liste.ElementAt(i).titre);
                            message = liste.ElementAt(i).titre;
                        }
                    }
                }
                log.Debug("Appel de la notification");
                // On affiche la notification
                NotificationWindow window = new NotificationWindow(message);
                window.Show();
            }
            log.Debug("Valeur de filtre interface : " + tbxFiltre.Text);
            // Si on a un filtre actif, on refresh pas la vue
            if(string.IsNullOrEmpty(tbxFiltre.Text))
            {
                // On garde la sélection dans la liste
                int Selected = listBox1.SelectedIndex;
                this.ListeAffichee = parser.GetList(string.Empty);
                log.Debug("Nombre d'items liste affichée : " + this.ListeAffichee.Count);
                listBox1.ItemsSource = null;
                listBox1.ItemsSource = this.ListeAffichee;

                if(Selected != -1)
                    listBox1.SelectedItem = listBox1.Items.GetItemAt(Selected);

                // Et on met à jour le nombre d'items affichés
                lnNbItems.Content = listBox1.Items.Count + " élement(s).";
            }
            UpdateText.Visibility = Visibility.Hidden;
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            log.Debug("Sélection d'un item - Index = " + listBox1.SelectedIndex);
            // Si une ligne est sélectionnée dans la liste, on initialise les champs avec la valeur du deal
            if (listBox1.SelectedIndex != -1)
            {
                ItemSelectionne = this.ListeAffichee.ElementAt(listBox1.SelectedIndex);
                lbTitre.Content = ItemSelectionne.titre;
                lbDate.Content = ItemSelectionne.date.ToString();

                ImageDeal.Source = null;
                Tbk_Code.Text = string.Empty;

                LancerWorkerParsing();

                // On empêche aussi le clic sur le bouton "Ouvrir"
                btnOuvrirUrl.IsEnabled = true;
                UpdateItemAffiche();
            }
            // Sinon, on vide les champs
            else
            {
                btnOuvrirUrl.IsEnabled = false;
                lbTitre.Content = string.Empty;
                lbDate.Content = string.Empty;
            }
        }

        private void LancerWorkerParsing()
        {
            BackgroundWorker WorkerParsing = new BackgroundWorker();
            WorkerParsing.DoWork += WorkerParsing_DoWork;
            WorkerParsing.RunWorkerCompleted += WorkerParsing_End;
            WorkerParsing.RunWorkerAsync();
        }

        private void WorkerParsing_DoWork(object sender, DoWorkEventArgs e)
        {
            if (string.IsNullOrEmpty(ItemSelectionne.LinkImage))
            {
                log.Debug("Pas d'image dans l'item. Parsing de l'item...");
                this.ItemSelectionne.ParserCode();
                this.ItemSelectionne.ParserImage();
                this.ItemSelectionne.ParserUrlDeal();
            }
            this.ItemSelectionne.ParserDegre();
        }

        private void WorkerParsing_End(object sender, RunWorkerCompletedEventArgs e)
        {
            // On lance la méthode de MAJ de l'item
            UpdateItemAffiche();
        }

        private void UpdateItemAffiche()
        {
            Tbk_Code.Text = ItemSelectionne.Code;
            // On cherche à parser l'image seulement si il en a une
            if (!string.IsNullOrEmpty(ItemSelectionne.LinkImage))
            {
                BitmapImage BImageDeal = new BitmapImage();
                BImageDeal.BeginInit();
                BImageDeal.UriSource = new Uri(ItemSelectionne.LinkImage, UriKind.Absolute);
                BImageDeal.EndInit();
                ImageDeal.Source = BImageDeal;
            }
            else
            {
                ImageDeal.Source = null;
            }
            lbHot.Content = ItemSelectionne.Degre;
            Btn_OuvrirDealExterne.IsEnabled = !string.IsNullOrEmpty(ItemSelectionne.UrlDealabs);
        }

        private void btnActualiser_Click(object sender, RoutedEventArgs e)
        {
            UpdateText.Visibility = Visibility.Visible;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.WorkerReportsProgress = true;
            worker.RunWorkerCompleted += worker_UpdateUI;
            worker.RunWorkerAsync();
        }

        private void listBox1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Si on a une sélection
            if (listBox1.SelectedIndex != -1)
            {
                // On ouvre la page du deal
                OuvrirURL();
            }
        }


        private void ShowNotif()
        {
            NotificationWindow notif = new NotificationWindow("Nouveaux élements");
            notif.ShowDialog();
            Thread.Sleep(10000);
            notif.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void tbxFiltre_TextChanged(object sender, TextChangedEventArgs e)
        {
            string ContentFiltre = tbxFiltre.Text;
            // On filtre directement sur le contenu de la liste
            this.ListeAffichee = parser.GetList(ContentFiltre);
            // Et on l'affiche
            this.listBox1.SelectedIndex = -1;
            this.listBox1.ItemsSource = this.ListeAffichee;
            lnNbItems.Content = listBox1.Items.Count + " élement(s).";
        }

        private void btnOuvrirUrl_Click(object sender, RoutedEventArgs e)
        {
            // Si on a une sélection
            if (listBox1.SelectedIndex != -1)
            {
                // On ouvre la page du deal
                OuvrirURL();
            }
        }

        private void Btn_OuvrirDealExterne_Click(object sender, RoutedEventArgs e)
        {
            // Si on a une sélection
            if (listBox1.SelectedIndex != -1)
            {
                string Url = this.ListeAffichee.ElementAt(listBox1.SelectedIndex).ParserUrlDeal();
                Process.Start(Url);
            }
        }

        private void OuvrirURL()
        {
            // Si on a un filtre actif, charge l'item dans la liste des items filtrés
            string Url;
            if (!string.IsNullOrEmpty(tbxFiltre.Text))
            {
                Url = this.ListeAffichee.ElementAt(listBox1.SelectedIndex).UrlDealabs;
            }
            else
            {
                Url = this.ListeAffichee.ElementAt(listBox1.SelectedIndex).UrlDealabs;
            }
            Process.Start(Url);
        }
    }
}
