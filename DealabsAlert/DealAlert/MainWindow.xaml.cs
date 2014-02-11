﻿using System;
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
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Shell;

namespace DealAlert
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class DealabsAlert : Window
    {

        DealabsParser parser;
        public DealabsAlert()
        {
            InitializeComponent();
            parser = new DealabsParser(ConfigurationSettings.AppSettings["url"], Convert.ToInt16(ConfigurationSettings.AppSettings["refreshMinutes"]));
            parser.updateItems(false);
            this.listBox1.ItemsSource = parser.AlllistItems;
            
            ItemNotifier.LeParser = parser;
            Thread newThread = new Thread(ItemNotifier.ThreadLoop);
            newThread.SetApartmentState(ApartmentState.STA);
            newThread.Start();
            newThread.IsBackground = true;

            lnNbItems.Content = listBox1.Items.Count + " élement(s).";
            btnOuvrirUrl.IsEnabled = false;

            
            JumpTask test = new JumpTask();
            test.Title = "Test ?";
            test.Description = "Jump de test";
            test.ApplicationPath = "www.google.fr";
            JumpList list = new JumpList();
            list.JumpItems.Add(test);
            JumpList.SetJumpList(System.Windows.Application.Current, list);
            list.Apply();
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                lbTitre.Content = parser.listItemsAffichee.ElementAt(listBox1.SelectedIndex).titre;
                lbDate.Content = parser.listItemsAffichee.ElementAt(listBox1.SelectedIndex).date.ToString();
                btnOuvrirUrl.IsEnabled = true;
            }
            else
            {
                btnOuvrirUrl.IsEnabled = false;
                lbTitre.Content = string.Empty;
                lbDate.Content = string.Empty;
            }
        }

        private void btnActualiser_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Wait;
            parser.updateItems(false);
            this.listBox1.SelectedIndex = -1;
            listBox1.ItemsSource = parser.AlllistItems;
            this.Cursor = null;
            lnNbItems.Content = listBox1.Items.Count + " élement(s).";
        }

        private void btnFiltre_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbxFiltre.Text))
            {
                parser.filtrerItems(tbxFiltre.Text);    
            }
            this.listBox1.SelectedIndex = -1;
            this.listBox1.ItemsSource = parser.listItemsFiltres;
            lnNbItems.Content = listBox1.Items.Count + " élement(s).";
            tbxFiltre.Text = string.Empty;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            parser.resetFiltre();
            this.listBox1.SelectedIndex = -1;
            listBox1.ItemsSource = parser.AlllistItems;
            lnNbItems.Content = listBox1.Items.Count + " élement(s).";
        }

        private void listBox1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string Url = parser.listItemsAffichee.ElementAt(listBox1.SelectedIndex).url;
                Process.Start(Url);
            }
        }

        private void btnOuvrirUrl_Click(object sender, RoutedEventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string Url = parser.listItemsAffichee.ElementAt(listBox1.SelectedIndex).url;
                Process.Start(Url);
            }
        }

        private void ShowNotif()
        {
            NotificationWindow notif = new NotificationWindow("Nouveaux élements");
            notif.ShowDialog();
            Thread.Sleep(10000);
            notif.Close();
        }
    }
}
