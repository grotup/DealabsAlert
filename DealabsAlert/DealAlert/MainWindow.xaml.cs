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

namespace DealAlert
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class DealabsAlert : Window
    {
        DealabsParser parser;
        DateTime DateDernierItem;

        public DealabsAlert()
        {
            InitializeComponent();
            parser = new DealabsParser(ConfigurationSettings.AppSettings["url"], Convert.ToInt16(ConfigurationSettings.AppSettings["refreshMinutes"]));
            parser.updateItems(false);
            this.listBox1.ItemsSource = parser.AlllistItems;
            
            lnNbItems.Content = listBox1.Items.Count + " élement(s).";
            btnOuvrirUrl.IsEnabled = false;

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
            DateDernierItem = parser.AlllistItems.ElementAt(0).date;
            parser.updateItems(true);
        }

        private void worker_UpdateUI(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((parser.DateDernierItem.CompareTo(DateDernierItem) > 0))
            {
                List<DealabsItem> liste = parser.getNouveauxDeals(DateDernierItem);
                // Si on a un filtre de défini, on boucle, sinon, on pop juste le nombre d'items nouveaux
                string filtre = ConfigurationSettings.AppSettings["filtre"];
                // On définit le message à comme étant le nombre de nouveaux deals
                string message = liste.Count + " nouveaux deal(s) !";
                // Si on a un filtre, on boucle
                if (!string.IsNullOrEmpty(filtre))
                {
                    for (int i = 0; i < liste.Count; i++)
                    {
                        // Si on trouve un item, on définit le message comme étant le titre du deal
                        if (liste.ElementAt(i).titre.Contains(filtre))
                        {
                            message = liste.ElementAt(i).titre;
                        }
                    }
                }
                // On affiche la notification
                NotificationWindow window = new NotificationWindow(message);
                window.Show();
            }
            // Si on a un filtre actif, on refresh pas la vue
            if(string.IsNullOrEmpty(tbxFiltre.Text))
            {
                // On garde la sélection dans la liste
                int Selected = listBox1.SelectedIndex;
                listBox1.ItemsSource = parser.AlllistItems;

                if(Selected != -1)
                    listBox1.SelectedItem = listBox1.Items.GetItemAt(Selected);

                // Et on met à jour le nombre d'items affichés
                lnNbItems.Content = listBox1.Items.Count + " élement(s).";
            }
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Si une ligne est sélectionnée dans la liste, on initialise les champs avec la valeur du deal
            if (listBox1.SelectedIndex != -1)
            {
                lbTitre.Content = parser.listItemsAffichee.ElementAt(listBox1.SelectedIndex).titre;
                lbDate.Content = parser.listItemsAffichee.ElementAt(listBox1.SelectedIndex).date.ToString();
                // On empêche aussi le clic sur le bouton "Ouvrir"
                btnOuvrirUrl.IsEnabled = true;
            }
            // Sinon, on vide les champs
            else
            {
                btnOuvrirUrl.IsEnabled = false;
                lbTitre.Content = string.Empty;
                lbDate.Content = string.Empty;
            }
        }

        private void btnActualiser_Click(object sender, RoutedEventArgs e)
        {
            // On change le curseur
            this.Cursor = System.Windows.Input.Cursors.Wait;
            // On lance l'update
            parser.updateItems(false);
            // Et on met à jour la vue
            this.listBox1.SelectedIndex = -1;
            listBox1.ItemsSource = parser.AlllistItems;
            this.Cursor = null;
            lnNbItems.Content = listBox1.Items.Count + " élement(s).";
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

        private void btnOuvrirUrl_Click(object sender, RoutedEventArgs e)
        {
            // Si on a une sélection
            if (listBox1.SelectedIndex != -1)
            {
                // On ouvre la page du deal
                OuvrirURL();
            }
        }

        private void OuvrirURL()
        {
            // Si on a un filtre actif, charge l'item dans la liste des items filtrés
            string Url;
            if (!string.IsNullOrEmpty(tbxFiltre.Text))
            {
                Url = parser.listItemsFiltres.ElementAt(listBox1.SelectedIndex).url;
            }
            else
            {
                Url = parser.AlllistItems.ElementAt(listBox1.SelectedIndex).url;
            }
            Process.Start(Url);
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
            parser.filtrerItems(ContentFiltre);
            // Et on l'affiche
            this.listBox1.SelectedIndex = -1;
            this.listBox1.ItemsSource = parser.listItemsFiltres;
            lnNbItems.Content = listBox1.Items.Count + " élement(s).";
        }
    }
}
