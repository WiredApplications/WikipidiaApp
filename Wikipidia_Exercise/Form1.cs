using System.Net;
using Newtonsoft.Json;
using System.Speech.Synthesis;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.IO;
using System.Collections.Generic;



namespace Wikipidia_Exercise
{
    public partial class Form1 : Form
    {
        SpeechSynthesizer speaker = new SpeechSynthesizer();
        string language = "el";
        FavSearch selectedSearch;
        public Form1()
        {
            InitializeComponent();
            
            this.AutoScaleMode = AutoScaleMode.None;
            this.ClientSize = new Size(964, 617);
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;

            textBox3.ScrollBars = ScrollBars.Vertical;
            /*
            //DEBUGGING 
            var synth = new SpeechSynthesizer();
            var voices = synth.GetInstalledVoices();

            MessageBox.Show($"Voices found: {voices.Count}");

            foreach (InstalledVoice v in voices)
            {
                var info = v.VoiceInfo;

                MessageBox.Show(
                    $"Name: {info.Name}\n"
                );
            }  
            */
            speaker.SelectVoice("Microsoft Stefanos");
            
            speaker.Rate = 0;     
            speaker.Volume = 100; 

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            
            string input = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Type something");
                return;
            }
            UIManaging(false);
            await LoadWikiData(input);
        }

        private async Task LoadWikiData(string term)
        {
            string url = $"https://{language}.wikipedia.org/api/rest_v1/page/summary/{Uri.EscapeDataString(term)}";



            using (HttpClient client = new HttpClient())
            {
                // REQUIRED by Wikipedia
                client.DefaultRequestHeaders.UserAgent.ParseAdd(
                    "MiniWiki1/1.0 (WinForms student project)"
                );

                HttpResponseMessage response;

                try
                {
                    response = await client.GetAsync(url);
                }
                catch (HttpRequestException)
                {
                    MessageBox.Show("No internet connection");
                    return;
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Article not found");
                    return;
                }

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Retrieve error: {response.StatusCode}");
                    return;
                }

                string json = await response.Content.ReadAsStringAsync();

                WikiSummary data;

                try
                {
                    data = JsonConvert.DeserializeObject<WikiSummary>(json);
                }
                catch (JsonException)
                {
                    MessageBox.Show("Error parsing data");
                    return;
                }

                UpdateUI(data);
            }
        }

        private async void UpdateUI(WikiSummary data)
        {
            label2.Text = data.title;
            textBox3.Text = data.extract;

            if (data.thumbnail != null && !string.IsNullOrEmpty(data.thumbnail.source))
            {
                await LoadImageAsync(data.thumbnail.source);
            }
            else
            {
                pictureBox1.Image = Properties.Resources.images_not_found_4012228725;
            }
        }
        private async Task LoadImageAsync(string imageUrl)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(
                        "MiniWiki1/1.0 (WinForms student project)"
                    );

                    HttpResponseMessage response = await client.GetAsync(imageUrl);

                    if (!response.IsSuccessStatusCode)
                        throw new Exception("HTTP error");

                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        pictureBox1.Image = Image.FromStream(ms);
                    }
                }
            }
            catch
            {
                
                pictureBox1.Image = Properties.Resources.images_not_found_4012228725;
            }
        }


        private void label3_Click(object sender, EventArgs e)
        {
            language = "en";
            speaker.SelectVoice("Microsoft David Desktop");
        }

        private void label4_Click(object sender, EventArgs e)
        {
            language = "ja";
        }

        private void label5_Click(object sender, EventArgs e)
        {
            language = "ru";
        }

        private void label6_Click(object sender, EventArgs e)
        {
            language = "el";
            speaker.SelectVoice("Microsoft Stefanos");
        }

        private void label11_Click(object sender, EventArgs e)
        {
            UIManaging(true);
        }
        private void UIManaging(Boolean Home)
        {
            label3.Visible = Home;
            label4.Visible = Home;
            label5.Visible = Home;
            label6.Visible = Home;
            label7.Visible = Home;
            label8.Visible = Home;
            label9.Visible = Home;
            label10.Visible = Home;
            pictureBox2.Visible = Home;
            textBox3.Visible = !Home;
            textBox3.Visible = !Home;
            pictureBox1.Visible = !Home;
            label2.Visible = !Home;
            button2.Visible = !Home;
            button3.Visible = !Home;
            dataGridView1.Visible = false;
            button4.Visible = false;
        }

        private void label3_MouseEnter(object sender, EventArgs e)
        {
            label3.Font = new Font(label3.Font, FontStyle.Underline);
        }

        private void label3_MouseLeave(object sender, EventArgs e)
        {
            label3.Font = new Font(label3.Font, FontStyle.Regular);
        }

        private void label4_MouseEnter(object sender, EventArgs e)
        {
            label4.Font = new Font(label4.Font, FontStyle.Underline);
        }

        private void label4_MouseLeave(object sender, EventArgs e)
        {
            label4.Font = new Font(label4.Font, FontStyle.Regular);
        }

        private void label5_MouseEnter(object sender, EventArgs e)
        {
            label5.Font = new Font(label5.Font, FontStyle.Underline);
        }

        private void label5_MouseLeave(object sender, EventArgs e)
        {
            label5.Font = new Font(label5.Font, FontStyle.Regular);
        }

        private void label6_MouseEnter(object sender, EventArgs e)
        {
            label6.Font = new Font(label6.Font, FontStyle.Underline);
        }

        private void label6_MouseLeave(object sender, EventArgs e)
        {
            label6.Font = new Font(label6.Font, FontStyle.Regular);
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Nothing to read.");
                return;
            }

            speaker.SpeakAsyncCancelAll(); 
            speaker.SpeakAsync(textBox3.Text);
        }

        private void label12_MouseEnter(object sender, EventArgs e)
        {
            label12.Font = new Font(label12.Font, FontStyle.Underline);
        }

        private void label12_MouseLeave(object sender, EventArgs e)
        {
            label12.Font = new Font(label12.Font, FontStyle.Bold);
        }

        

        private void label11_MouseLeave(object sender, EventArgs e)
        {
            label11.Font = new Font(label11.Font, FontStyle.Bold);
        }

        private void label11_MouseEnter(object sender, EventArgs e)
        {
            label11.Font = new Font(label11.Font, FontStyle.Underline);
        }
        private async Task SaveFavourite(string searchTerm, string lang)
        {
            using (HttpClient client = new HttpClient())
            {
                Supabase supabase = new Supabase();

                client.DefaultRequestHeaders.Add("apikey", supabase.ApiKey);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabase.ApiKey}");

                var data = new
                {
                    Name = searchTerm,
                    Language = lang
                };

                string json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(
                    $"{supabase.SupabaseUrl}/rest/v1/Searches",
                    content
                );

                if (!response.IsSuccessStatusCode)
                {
                    //Debugging
                   // string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(
                       // $"Status: {response.StatusCode}\n\n{error}",
                        "Supabase error"
                    );
                }
            }
        }

        private async Task LoadFavouritesAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                Supabase supabase = new Supabase();

                client.DefaultRequestHeaders.Add("apikey", supabase.ApiKey.Trim());
                client.DefaultRequestHeaders.Add(
                    "Authorization",
                    $"Bearer {supabase.ApiKey.Trim()}"
                );

                HttpResponseMessage response = await client.GetAsync(
                    $"{supabase.SupabaseUrl}/rest/v1/Searches?select=Name,Language"
                );

                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Load failed:\n{error}");
                    return;
                }

                string json = await response.Content.ReadAsStringAsync();

                List<FavSearch> data =
                    JsonConvert.DeserializeObject<List<FavSearch>>(json);

                dataGridView1.DataSource = data;
            }
        }
        private async void dataGridView1_CellDoubleClick( object sender, DataGridViewCellEventArgs e)
        {
           
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            FavSearch selectedSearch1 = new FavSearch
            {
                Name = row.Cells["Name"].Value.ToString(),
                Language = row.Cells["Language"].Value.ToString()
            };
           

            
             language = selectedSearch1.Language;



            UIManaging(false);
            await LoadWikiData(selectedSearch1.Name);
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
                return;
            button4.Visible = true;
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            selectedSearch = new FavSearch
            {
                Name = row.Cells["Name"].Value.ToString(),
                Language = row.Cells["Language"].Value.ToString()
            };  
          
            
        }



        private async void button3_Click(object sender, EventArgs e)
        {
           await SaveFavourite(textBox2.Text.Trim(),language);
        }

        private void label11_Click_1(object sender, EventArgs e)
        {
            UIManaging(true);
            textBox3.Text = "";
            pictureBox1.Image = null;
        }

        

        private async void label12_Click(object sender, EventArgs e)
        {
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            label10.Visible = false;
            pictureBox2.Visible = true;
            textBox3.Visible = false;
            textBox3.Visible = false;
            pictureBox1.Visible = false;
            label2.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            dataGridView1.Visible = true;
            selectedSearch = null;
            await LoadFavouritesAsync();
        }
        public static async Task<bool> DeleteFavourite(FavSearch Delsearch)
        {
            try
            {
                Supabase supabase = new Supabase();

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("apikey", supabase.ApiKey.Trim());
                    client.DefaultRequestHeaders.Add(
                        "Authorization",
                        $"Bearer {supabase.ApiKey.Trim()}"
                    );

                    string url =
                        $"{supabase.SupabaseUrl}/rest/v1/Searches" +
                        $"?Name=eq.{Uri.EscapeDataString(Delsearch.Name)}" +
                        $"&Language=eq.{Uri.EscapeDataString(Delsearch.Language)}";

                    HttpResponseMessage response =
                        await client.DeleteAsync(url);

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Supabase delete error");
                return false;
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            if (selectedSearch != null)
            {
                await DeleteFavourite(selectedSearch);
            }
            else
            {
                MessageBox.Show("No search selected.");
                return;
            }
           await LoadFavouritesAsync();

        }
    }
    
    }



    public class WikiSummary
    {
        public string title { get; set; }
        public string extract { get; set; }
        public Thumbnail thumbnail { get; set; }
    }

    public class Thumbnail
    {
        public string source { get; set; }
        
    }
public class Supabase
{
    public readonly string SupabaseUrl =
        "https://hbxldnvyfaczymionfty.supabase.co";

    public readonly string ApiKey =
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImhieGxkbnZ5ZmFjenltaW9uZnR5Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjgxMjIwNzMsImV4cCI6MjA4MzY5ODA3M30.KwQfSKrStOY-AiAgrjkDnYnzjHqd8OPLWBhmBQEvRjc";
}
public class FavSearch
{
    public string Name { get; set; }
    public string Language { get; set; }
}

