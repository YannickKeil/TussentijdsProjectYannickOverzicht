using System;
using System.Collections.Generic;
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
            var Personeel = dc.Personeelslids.Select(p => new { Voornaam = p.Voornaam, Achternaam = p.Achternaam, AdminRecht = p.AdminRechten.titel, Username = p.Username, DatumIntreding = p.Indiensttreding, Geboortedatum = p.GeboorteDatum });
            dgPersoneel.ItemsSource = Personeel;
            var klant = dc.Klants.Select(k => new { Voornaam = k.Voornaam, Achternaam = k.Achternaam, Straatnaam = k.Straatnaam, Huisnummer = k.Huisnummer, Bus = k.Bus, Postcode = k.Postcode, Gemeente = k.Gemeente, Telefoon = k.Telefoonnummer, Emailadress = k.Emailadres, AangemaaktOp = k.AangemaaktOp, Opmerking = k.Opmerking });
            dgKlant.ItemsSource = klant;
            var Leverancier = dc.Leveranciers.Select(l => new { Contactpersoon = l.Contactpersoon, Telefoon = l.Telefoonnummer, Email = l.Emailadres, Straatnaam = l.Straatnaam, Huisnummer = l.Huisnummer, Bus = l.Bus, Postcode = l.Postcode, Gemeente = l.Gemeente });
            dgLeverancier.ItemsSource = Leverancier;
            var bestellingen = dc.BestellingProducts.Select(bp => new { KlantLeverancierTest = /*bp.Bestelling.Leverancier.Contactpersoon + dit geeft moeilijkheden voor vestelgeschiedenis*/ bp.Bestelling.Klant.Voornaam + " " + bp.Bestelling.Klant.Achternaam, BestellingsNummer = bp.BestellingID, DateOpmaak = bp.Bestelling.DatumOpgemaakt.ToShortDateString(), Personeel = bp.Bestelling.Personeelslid.Voornaam + " " + bp.Bestelling.Personeelslid.Achternaam, Producten = dc.BestellingProducts.Where(x => x.BestellingID == bp.BestellingID).Select(x => x.AantalProtuctBesteld + " " + x.Product.Naam + " " + x.Product.Eenheid + " €" + x.Product.AankoopPrijs) })/*.GroupBy(x => x.BestellingsNummer)*/;
            ListViewBestellingen.ItemsSource = bestellingen.ToList();
            
            Bestelgeschiedenis.ItemsSource = bestellingen.ToList();
            //BestelGesKlantLeverancier.Header = "Klant";
        }

        private void TabOverzicht_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
