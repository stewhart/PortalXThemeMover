using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Linq;
using StarRezApi;

namespace PortalXThemeMover
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
           


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            comboBoxListOfThemes.Items.Clear();
           QuerySourceTemplates();
        }

        public void QuerySourceTemplates()
        {
            StarRezApiClient sourceClient = new StarRezApiClient(textBoxRestAdd1.Text, textBoxUser1.Text, textBoxPass1.Text);
            try
            {
                var SourceTemplates = sourceClient.Query("SELECT * FROM PortalTheme");



                String test = SourceTemplates.ElementAt(2).Description.ToString();

                globalclass.ThemeType = UpdateDropDown(SourceTemplates);
                Console.WriteLine("test");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
            }

        }

        public Dictionary<string, string> UpdateDropDown(dynamic[] ThemesToShow)
        {
            dynamic results = ThemesToShow;
            int themeCounter = 0;
            comboBoxListOfThemes.DisplayMember = "Text";
            comboBoxListOfThemes.ValueMember = "Value";
            Dictionary<string, string> ThemeTypeCalc = new Dictionary<string, string>();
            foreach (ApiObject theme in results)
            {
                Console.WriteLine(results[themeCounter].Description.ToString());
                comboBoxListOfThemes.Items.Add(new { Text = results[themeCounter].Description.ToString(), Value = results[themeCounter].PortalThemeID.ToString() });
                ThemeTypeCalc.Add(results[themeCounter].PortalThemeID.ToString(), results[themeCounter].ThemeLayout.ToString());
                themeCounter++;
            }

            return ThemeTypeCalc;

        }

        private void buttonCopyTheme_Click(object sender, EventArgs e)
        {
            //Console.WriteLine(globalclass.ThemeType.First().ToString());
            int TempID = Int32.Parse(((comboBoxListOfThemes.SelectedItem as dynamic).Value));
            Console.WriteLine(TempID.ToString());
            QueryGetTemplateSetting(TempID);

        }

        public void QueryGetTemplateSetting(int ThemeID)
        {
            string getThemeSettings = @"SELECT * FROM PortalSetting WHERE TableName = ""PortalTheme"" AND TableID = " + ThemeID;
            StarRezApiClient sourceClient = new StarRezApiClient(textBoxRestAdd1.Text, textBoxUser1.Text, textBoxPass1.Text);
            var SourceSettings = sourceClient.Query(getThemeSettings);
           // String test = SourceSettings.ElementAt(2).Description.ToString();
            Console.WriteLine("test");
            int newThemeID = createTheme(ThemeID);
            addSettings(SourceSettings, newThemeID);

        }

        public void addSettings(dynamic settingList, int targetThemeID)
        {
            int settingCounter = 0;
            StarRezApiClient settingClient = new StarRezApiClient(textBoxRestAdd2.Text, textBoxUser2.Text, textBoxPass2.Text);
            var newSetting = settingClient.CreateDefault("PortalSetting");
            foreach (ApiObject theme in settingList)
            {
                newSetting.TableID = targetThemeID;
                newSetting.TableName = "PortalTheme";
                newSetting.Description = settingList[settingCounter].Description;
                newSetting.Value = settingList[settingCounter].Value;


                Console.WriteLine(settingList[settingCounter].Description.ToString());
                //  comboBoxListOfThemes.Items.Add(new { Text = results[themeCounter].Description.ToString(), Value = results[themeCounter].PortalThemeID.ToString() });
                settingClient.Create(newSetting);
                settingCounter++;
            }
            MessageBox.Show("Settings created... hopefully");
        }

        public int createTheme(int ThemeID)
        {
            StarRezApiClient createThemeClient = new StarRezApiClient(textBoxRestAdd2.Text, textBoxUser2.Text, textBoxPass2.Text);
            var newTheme = createThemeClient.CreateDefault("PortalTheme");
            newTheme.Description = ThemeID.ToString() + "-" + ((comboBoxListOfThemes.SelectedItem as dynamic).Text);
            string result;
            globalclass.ThemeType.TryGetValue(ThemeID.ToString(), out result);
            newTheme.ThemeLayout = result;
            int newThemeID = createThemeClient.Create(newTheme);
            Console.WriteLine(newThemeID.ToString());
            return newThemeID;

        }
        static class globalclass
        {
            public static Dictionary<string, string> ThemeType = new Dictionary<string, string>();
        }

    }



}
