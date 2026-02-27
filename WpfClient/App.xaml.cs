using System.Configuration;
using System.Data;
using System.Windows;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void ChangeLanguage(string culture)
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (culture)
            {
                case "en":
                    dict.Source = new Uri("Languages/StringResources.en.xaml", UriKind.Relative);
                    break;
                case "sr":
                    dict.Source = new Uri("Languages/StringResources.sr.xaml", UriKind.Relative);
                    break;
            }

            // Brišemo stari rečnik i dodajemo novi
            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(dict);
        }
    }

}
