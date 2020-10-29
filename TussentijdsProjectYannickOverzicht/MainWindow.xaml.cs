using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TussentijdsProjectYannickOverzicht
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataClasses1DataContext dc = new DataClasses1DataContext(Properties.Settings.Default.Projectweek_YannickConnectionString);
        public MainWindow()
        {
            InitializeComponent();
            var Personeel = dc.Personeelslids.Select(p => new { ID = p.PersoneelslidID, Voornaam = p.Voornaam, Achternaam = p.Achternaam, AdminRecht = p.AdminRechten.titel, Username = p.Username, DatumIntreding = p.Indiensttreding.ToShortDateString(), Geboortedatum = p.GeboorteDatum.ToShortDateString() });
            lvPersoneel.SelectedValuePath = "ID";
            lvPersoneel.ItemsSource = Personeel;
            var klant = dc.Klants.Select(k => new { ID = k.KlantID, Voornaam = k.Voornaam, Achternaam = k.Achternaam, Straatnaam = k.Straatnaam, Huisnummer = k.Huisnummer, Bus = k.Bus, Postcode = k.Postcode, Gemeente = k.Gemeente, Telefoon = k.Telefoonnummer, Emailadress = k.Emailadres, AangemaaktOp = k.AangemaaktOp.ToShortDateString(), Opmerking = k.Opmerking });
            lvKlant.SelectedValuePath = "ID";
            lvKlant.ItemsSource = klant;
            var Leverancier = dc.Leveranciers.Select(l => new { ID = l.LeverancierID ,Contactpersoon = l.Contactpersoon, Telefoon = l.Telefoonnummer, Emailadress = l.Emailadres, Straatnaam = l.Straatnaam, Huisnummer = l.Huisnummer, Bus = l.Bus, Postcode = l.Postcode, Gemeente = l.Gemeente });
            lvLeverancier.SelectedValuePath = "ID";
            lvLeverancier.ItemsSource = Leverancier;
            var bestellingen = dc.BestellingProducts.Select(bp => new { KlantNaam = bp.Bestelling.Klant.Voornaam + " " + bp.Bestelling.Klant.Voornaam, LeverancierContact = bp.Bestelling.Leverancier.Contactpersoon, BestellingsNummer = bp.BestellingID, DateOpmaak = bp.Bestelling.DatumOpgemaakt.ToShortDateString(), Personeel = bp.Bestelling.Personeelslid.Voornaam + " " + bp.Bestelling.Personeelslid.Achternaam, Producten = dc.BestellingProducts.Where(x => x.BestellingID == bp.BestellingID).Select(x => new { AantalProducten = x.AantalProtuctBesteld, ProductNaam = x.Product.Naam, ProductEenheid = x.Product.Eenheid, Netto = $"€ {x.Product.Netto()}", Brutto = $"€ {x.Product.Bruto()}" }) }).GroupBy(x => x.BestellingsNummer);
            lvBestellingen.ItemsSource = bestellingen.ToList();

            
            var productenOverview = dc.Products.Select(p => new { TotaalVerkocht = dc.BestellingProducts.Where(bp => bp.ProductID == p.ProductID && bp.Bestelling.KlantID != null).Select(bp => bp.AantalProtuctBesteld).Sum(), WinstPerProduct = p.Winst(), ProductID = p.ProductID, Naam = p.Naam, Marge = p.Marge, Eenheid = p.Eenheid, BTW = p.BTW, LeverancierNaam = p.Leverancier.Contactpersoon, CategorieNaam = p.Categorie.CategorieNaam, AantalOpVooraad = p.AantalOpVooraad,AankoopPrijs =p.AankoopPrijs , Netto = p.Netto() , Brutto = p.Bruto() }).ToList();
            
            foreach (var item in productenOverview)
            {
                POverview.Add(new ProductenOverview(item.ProductID, item.Naam, item.Marge, item.Eenheid, item.BTW, item.LeverancierNaam, item.CategorieNaam, item.AantalOpVooraad, item.AankoopPrijs, item.Netto, item.Brutto, item.WinstPerProduct, item.WinstPerProduct * item.TotaalVerkocht, item.TotaalVerkocht));
            }
            //BestelGesKlantLeverancier.Header = "Klant";
            lvProducten.SelectedValuePath = "ProductID";
            lvProducten.ItemsSource = productenOverview.ToList();

            //var Category = dc.Categories.Select(c => new { ID = c.CategorieID, Name = c.CategorieNaam });
            //lvCategories.SelectedValuePath = "ID";
            //lvCategories.ItemsSource = Category.ToList();
            lvPersoneel.SelectedIndex = 0;
        }


        private void TabOverzicht_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {              
                if (TabOverzicht.SelectedIndex == 0)
                {
                    Bestelgeschiedenis.Visibility = Visibility.Visible;
                    BestelGesKlantLeverancierPersoneelslid.Header = "Personeel";
                    Bestelgeschiedenis.ItemsSource = null;
                    lvPersoneel.SelectedIndex = 1;
                    lvPersoneel.SelectedIndex = 0;
                }
                else if (TabOverzicht.SelectedIndex == 1)
                {
                    Bestelgeschiedenis.Visibility = Visibility.Visible;
                    BestelGesKlantLeverancierPersoneelslid.Header = "Klant";
                    Bestelgeschiedenis.ItemsSource = null;
                    lvKlant.SelectedIndex = 1;
                    lvKlant.SelectedIndex = 0;
                }
                else if (TabOverzicht.SelectedIndex == 2)
                {
                    Bestelgeschiedenis.Visibility = Visibility.Visible;
                    BestelGesKlantLeverancierPersoneelslid.Header = "Leverancier";
                    Bestelgeschiedenis.ItemsSource = null;
                    lvLeverancier.SelectedIndex = 1;
                    lvLeverancier.SelectedIndex = 0;
                }
                //else if (TabOverzicht.SelectedIndex == 5)
                //{
                //    Bestelgeschiedenis.Visibility = Visibility.Visible;
                //    BestelGesKlantLeverancierPersoneelslid.Header = "Categorie";
                //    Bestelgeschiedenis.ItemsSource = null;
                //    lvCategories.SelectedIndex = 1;
                //    lvCategories.SelectedIndex = 0;
                //}
                else
                {
                    Bestelgeschiedenis.Visibility = Visibility.Hidden;
                    Bestelgeschiedenis.ItemsSource = null;
                }
            }
        }


        private void lvPersoneel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Bestelgeschiedenis.ItemsSource = null;
            var bestellingen1 = dc.BestellingProducts.Where(bp => bp.Bestelling.PersoneelslidID == (int)lvPersoneel.SelectedValue).Select(bp => new { KlantLeverancierPersoneelslid = bp.Bestelling.Personeelslid.Voornaam + " " + bp.Bestelling.Personeelslid.Achternaam, BestellingsNummer = bp.BestellingID, DateOpmaak = bp.Bestelling.DatumOpgemaakt.ToShortDateString(), Personeel = bp.Bestelling.Personeelslid.Voornaam + " " + bp.Bestelling.Personeelslid.Achternaam, Producten = dc.BestellingProducts.Where(x => x.BestellingID == bp.BestellingID).Select(x => new { AantalProducten = x.AantalProtuctBesteld, ProductNaam = x.Product.Naam, ProductEenheid = x.Product.Eenheid, Netto = $"€ {x.Product.Netto()}", Brutto = $"€ {x.Product.Bruto()}", AankoopPrijs = x.Product.AankoopPrijs }).ToList() }).GroupBy(x => x.BestellingsNummer);

            Bestelgeschiedenis.ItemsSource = bestellingen1.ToList();

        }

        private void lvKlant_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Bestelgeschiedenis.ItemsSource = null;
            var bestellingen2 = dc.BestellingProducts.Where(bp => bp.Bestelling.KlantID == (int)lvKlant.SelectedValue).Select(bp => new { KlantLeverancierPersoneelslid = bp.Bestelling.Klant.Voornaam + " " + bp.Bestelling.Klant.Achternaam, BestellingsNummer = bp.BestellingID, DateOpmaak = bp.Bestelling.DatumOpgemaakt.ToShortDateString(), Personeel = bp.Bestelling.Personeelslid.Voornaam + " " + bp.Bestelling.Personeelslid.Achternaam, Producten = dc.BestellingProducts.Where(x => x.BestellingID == bp.BestellingID).Select(x => new { AantalProducten = x.AantalProtuctBesteld, ProductNaam = x.Product.Naam, ProductEenheid = x.Product.Eenheid, Netto = $"€ {x.Product.Netto()}", Brutto = $"€ {x.Product.Bruto()}", AankoopPrijs = x.Product.AankoopPrijs }).ToList() }).GroupBy(x => x.BestellingsNummer);
            Bestelgeschiedenis.ItemsSource = bestellingen2.ToList();
        }
        private void lvLeverancier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Bestelgeschiedenis.ItemsSource = null;
            var bestellingen2 = dc.BestellingProducts.Where(bp => bp.Bestelling.LeverancierID == (int)lvLeverancier.SelectedValue).Select(bp => new { KlantLeverancierPersoneelslid = bp.Bestelling.Leverancier.Contactpersoon, BestellingsNummer = bp.BestellingID, DateOpmaak = bp.Bestelling.DatumOpgemaakt.ToShortDateString(), Personeel = bp.Bestelling.Personeelslid.Voornaam + " " + bp.Bestelling.Personeelslid.Achternaam, Producten = dc.BestellingProducts.Where(x => x.BestellingID == bp.BestellingID).Select(x => new { AantalProducten = x.AantalProtuctBesteld, ProductNaam = x.Product.Naam, ProductEenheid = x.Product.Eenheid, Netto = $"€ {x.Product.Netto()}", Brutto = $"€ {x.Product.Bruto()}" ,AankoopPrijs = x.Product.AankoopPrijs}).ToList() }).GroupBy(x => x.BestellingsNummer);
            Bestelgeschiedenis.ItemsSource = bestellingen2.ToList();
        }
        //private void lvCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    Bestelgeschiedenis.ItemsSource = null;
        //    var bestellingen2 = dc.BestellingProducts.Where(bp => bp.Product.CategorieID == (int)lvCategories.SelectedValue).Select(bp => new { KlantLeverancierPersoneelslid = bp.Product.Categorie.CategorieNaam, BestellingsNummer = bp.BestellingID, DateOpmaak = bp.Bestelling.DatumOpgemaakt.ToShortDateString(), Personeel = bp.Bestelling.Personeelslid.Voornaam + " " + bp.Bestelling.Personeelslid.Achternaam, Producten = dc.BestellingProducts.Where(x => x.BestellingID == bp.BestellingID).Select(x => new { AantalProducten = x.AantalProtuctBesteld, ProductNaam = x.Product.Naam, ProductEenheid = x.Product.Eenheid, Netto = $"€ {x.Product.Netto()}", Brutto = $"€ {x.Product.Bruto()}" }).ToList() }).GroupBy(x => x.BestellingsNummer);

        //    Bestelgeschiedenis.ItemsSource = bestellingen2.ToList();
        //}

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            string uri = $"mailto:{e.Uri}?subject=&amp;body=TestBody";
            Uri myUri = new Uri(uri);
            Process.Start(myUri.AbsoluteUri);
            e.Handled = true;
        }
        List<ProductenOverview> POverview = new List<ProductenOverview>();
        public class ProductenOverview
        {
            int ProductIDPO { get; set; }
            string NaamPO { get; set; }
            decimal MargePO { get; set; }
            string EenheidPO { get; set; }
            decimal BTWPO { get; set; }
            string LeverancierContact { get; set; }
            string CategorieNaam{get;set;}
            int AantalOpVooraadPO { get; set; }
            decimal AankoopPrijsPO { get; set; }
            double NettoPO { get; set; }
            double BrutoPO { get; set; }
            double WinstePerEenheid { get; set; }
            double TotaalWinst { get; set; }
            int TotaalVerkocht { get; set; }

            public ProductenOverview(int productIDPO, string naamPO, decimal margePO, string eenheidPO, decimal bTWPO, string leverancierContact, string categorieNaam, int aantalOpVooraadPO, decimal aankoopPrijsPO, double nettoPO, double brutoPO, double winstePerEenheid, double totaalWinst, int totaalVerkocht)
            {
                ProductIDPO = productIDPO;
                NaamPO = naamPO;
                MargePO = margePO;
                EenheidPO = eenheidPO;
                BTWPO = bTWPO;
                LeverancierContact = leverancierContact;
                CategorieNaam = categorieNaam;
                AantalOpVooraadPO = aantalOpVooraadPO;
                AankoopPrijsPO = aankoopPrijsPO;
                NettoPO = nettoPO;
                BrutoPO = brutoPO;
                WinstePerEenheid = winstePerEenheid;
                TotaalWinst = totaalWinst;
                TotaalVerkocht = totaalVerkocht;
            }
        }

    }
}
