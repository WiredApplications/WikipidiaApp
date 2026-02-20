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
using System.Linq;

namespace Wikipidia_Exercise
{
    public partial class Form1 : Form
    {
        SpeechSynthesizer speaker = new SpeechSynthesizer();
        string language = "gr";
        FavSearch selectedSearch;

        public Form1()
        {
            InitializeComponent();

            this.AutoScaleMode = AutoScaleMode.None;
            this.ClientSize = new Size(964, 617);
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;

            label4.Cursor = Cursors.Hand;
            label5.Cursor = Cursors.Hand;
            label6.Cursor = Cursors.Hand;
            label3.Cursor = Cursors.Hand;

            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;

            textBox3.ScrollBars = ScrollBars.Vertical;

            speaker.Rate = 0;
            speaker.Volume = 100;
        }

        // ======== SPEECH CHECK (ΜΟΝΟ ΓΙΑ ΑΝΑΓΝΩΣΗ) ========
        private bool TrySetVoiceForCurrentLanguage()
        {
            string culture = "";

            if (language == "gr")
                culture = "el-GR";
            else if (language == "en")
                culture = "en-US";
            else if (language == "ru")
                culture = "ru-RU";
            else if (language == "ja")
                culture = "ja-JP";

            if (culture == "")
                return false;

            var voice = speaker.GetInstalledVoices()
                .FirstOrDefault(v => v.VoiceInfo.Culture.Name == culture);

            if (voice == null)
                return false;

            speaker.SelectVoice(voice.VoiceInfo.Name);
            return true;
        }

        // =================================================

        private async void button1_Click(object sender, EventArgs e)
        {
            string input = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Type something");
                return;
            }

            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            label10.Visible = false;

            UIManaging(false);
            await LoadWikiData(input);
        }

        private async Task LoadWikiData(string term)
        {
            string url = $"https://{language}.wikipedia.org/api/rest_v1/page/summary/{Uri.EscapeDataString(term)}";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("MiniWiki1/1.0");

                HttpResponseMessage response;

                try
                {
                    response = await client.GetAsync(url);
                }
                catch
                {
                    MessageBox.Show("No internet connection");
                    return;
                }

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Article not found");
                    return;
                }

                string json = await response.Content.ReadAsStringAsync();
                WikiSummary data = JsonConvert.DeserializeObject<WikiSummary>(json);
                UpdateUI(data);
            }
        }

        private async void UpdateUI(WikiSummary data)
        {
            label2.Text = data.title;
            textBox3.Text = data.extract;

            if (data.thumbnail != null && !string.IsNullOrEmpty(data.thumbnail.source))
                await LoadImageAsync(data.thumbnail.source);
            else
                pictureBox1.Image = Properties.Resources.images_not_found_4012228725;
        }

        private async Task LoadImageAsync(string imageUrl)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("MiniWiki1/1.0");
                    byte[] img = await client.GetByteArrayAsync(imageUrl);

                    using (MemoryStream ms = new MemoryStream(img))
                        pictureBox1.Image = Image.FromStream(ms);
                }
            }
            catch
            {
                pictureBox1.Image = Properties.Resources.images_not_found_4012228725;
            }
        }

        // ================= LANGUAGE (ΧΩΡΙΣ ΕΛΕΓΧΟ SPEECH) =================
        private void label3_Click(object sender, EventArgs e)
        {
            language = "en";
            label11.Text = "Change Language(en)";
        }

        private void label4_Click(object sender, EventArgs e)
        {
            language = "ja";
            label11.Text = "Change Language(ja)";
        }

        private void label5_Click(object sender, EventArgs e)
        {
            language = "ru";
            label11.Text = "Change Language(ru)";
        }

        private void label6_Click(object sender, EventArgs e)
        {
            language = "el";
            label11.Text = "Change Language(gr)";
        }
        // =================================================================

        // ================== ΑΝΑΓΝΩΣΗ ==================
        private async void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text))
                return;

            speaker.SpeakAsyncCancelAll();

            if (!TrySetVoiceForCurrentLanguage())
            {
                MessageBox.Show(
                    "Δεν υπάρχει διαθέσιμη φωνή για ανάγνωση στη συγκεκριμένη γλώσσα.",
                    "Speech not available",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            string text = textBox3.Text;
            if (text.Length > 1500)
                text = text.Substring(0, 1500);

            speaker.SpeakAsync(text);
        }
        // ==============================================

        private void UIManaging(bool Home)
        {
            label3.Visible = Home;
            label4.Visible = Home;
            label5.Visible = Home;
            label6.Visible = Home;
            pictureBox2.Visible = Home;

            textBox3.Visible = !Home;
            pictureBox1.Visible = !Home;
            label2.Visible = !Home;
            button2.Visible = !Home;
            button3.Visible = !Home;

            dataGridView1.Visible = false;
            button4.Visible = false;
        }

        private async void label12_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = true;
            selectedSearch = null;
            await LoadFavouritesAsync();
            pictureBox1.Visible = false;

            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.GridColor = Color.FromArgb(230, 230, 230);

            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 246, 248);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 235, 245);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dataGridView1.RowTemplate.Height = 28;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
        }

        private async Task LoadFavouritesAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                Supabase s = new Supabase();

                client.DefaultRequestHeaders.Add("apikey", s.ApiKey);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {s.ApiKey}");

                var response = await client.GetAsync($"{s.SupabaseUrl}/rest/v1/Searches?select=Name,Language");
                var json = await response.Content.ReadAsStringAsync();

                dataGridView1.DataSource =
                    JsonConvert.DeserializeObject<List<FavSearch>>(json);
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await SaveFavourite(textBox2.Text.Trim(), language);
        }

        private async Task SaveFavourite(string term, string lang)
        {
            using (HttpClient client = new HttpClient())
            {
                Supabase s = new Supabase();

                client.DefaultRequestHeaders.Add("apikey", s.ApiKey);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {s.ApiKey}");

                FavSearch f = new FavSearch { Name = term, Language = lang };
                var json = JsonConvert.SerializeObject(f);

                await client.PostAsync(
                    $"{s.SupabaseUrl}/rest/v1/Searches",
                    new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
            }
        }

        private async void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridView1.Rows[e.RowIndex];
            language = row.Cells["Language"].Value.ToString();

            UIManaging(false);
            await LoadWikiData(row.Cells["Name"].Value.ToString());

            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            label10.Visible = false;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            var row = dataGridView1.SelectedRows[0];
            selectedSearch = new FavSearch
            {
                Name = row.Cells["Name"].Value.ToString(),
                Language = row.Cells["Language"].Value.ToString()
            };

            button4.Visible = true;
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            if (selectedSearch != null)
                await DeleteFavourite(selectedSearch);

            await LoadFavouritesAsync();
        }

        public static async Task<bool> DeleteFavourite(FavSearch f)
        {
            try
            {
                Supabase s = new Supabase();
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("apikey", s.ApiKey);
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {s.ApiKey}");

                    string url =
                        $"{s.SupabaseUrl}/rest/v1/Searches?Name=eq.{f.Name}&Language=eq.{f.Language}";

                    var res = await client.DeleteAsync(url);
                    return res.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
                e.SuppressKeyPress = true;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
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
        public readonly string SupabaseUrl = "https://ogzbkjsppoynmbwiehvk.supabase.co";
        public readonly string ApiKey = "sb_publishable_WURijhOqXnJlcHRrd41Qrg_0wDZvhI5";
    }

    public class FavSearch
    {
        public string Name { get; set; }
        public string Language { get; set; }
    }
}
