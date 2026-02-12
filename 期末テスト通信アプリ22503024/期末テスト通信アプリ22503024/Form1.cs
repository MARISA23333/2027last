using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace 期末テスト通信アプリ22503024
{
    public partial class Form1 : Form
    {
        private const string ConnectionString =
            "Server=172.16.2.26;Database=team3;User Id=tateno;Password=ae21215926;SslMode=Preferred;CharSet=utf8mb4;";

        private const string TableName = "team3";
        private const string IdColumn = "id";

        private string _messageColumnName;

        public Form1()
        {
            InitializeComponent();

            dgvMessages.AutoGenerateColumns = true;
            dgvMessages.DataSource = null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                TestConnection();
                _messageColumnName = DetectMessageColumnName();
                ReloadMessages();
                SelectFirstRowIfAny();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "起動失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            ReloadMessages();
            SelectFirstRowIfAny();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var message = (txtMessage.Text ?? string.Empty).Trim();
            if (message.Length == 0)
            {
                MessageBox.Show("留言を入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                EnsureMessageColumnReady();

                using (var conn = new MySqlConnection(ConnectionString))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();

                    var hasMessageColumn = ColumnExists(conn, TableName, "message");
                    if (hasMessageColumn && !string.Equals(_messageColumnName, "message", StringComparison.OrdinalIgnoreCase))
                    {
                        cmd.CommandText = $"INSERT INTO `{TableName}` (`{_messageColumnName}`, `message`) VALUES (@msg, @msg2);";
                        cmd.Parameters.AddWithValue("@msg", message);
                        cmd.Parameters.AddWithValue("@msg2", message);
                    }
                    else
                    {
                        cmd.CommandText = $"INSERT INTO `{TableName}` (`{_messageColumnName}`) VALUES (@msg);";
                        cmd.Parameters.AddWithValue("@msg", message);
                    }

                    cmd.ExecuteNonQuery();
                }

                txtMessage.Clear();
                ReloadMessages();
                SelectFirstRowIfAny();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "追加失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvMessages.Rows.Count == 0 || dgvMessages.CurrentRow == null)
            {
                MessageBox.Show("削除する行を選択してね。", "選択エラだー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!dgvMessages.Columns.Contains(IdColumn))
            {
                MessageBox.Show("id 列が見つかりません", "削除失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var idObj = dgvMessages.CurrentRow.Cells[IdColumn].Value;
            if (idObj == null || idObj == DBNull.Value)
            {
                MessageBox.Show("選択行の id が取得できません。", "削除失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var id = Convert.ToInt32(idObj);
            if (MessageBox.Show($"id={id} を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();

                    cmd.CommandText = $"DELETE FROM `{TableName}` WHERE `{IdColumn}` = @id;";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                ReloadMessages();
                SelectFirstRowIfAny();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "削除失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReloadMessages()
        {
            try
            {
                EnsureMessageColumnReady();

                using (var conn = new MySqlConnection(ConnectionString))
                using (var cmd = conn.CreateCommand())
                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    conn.Open();

                    cmd.CommandText = $"SELECT `{IdColumn}`, `{_messageColumnName}` FROM `{TableName}` ORDER BY `{IdColumn}` DESC;";

                    var dt = new DataTable();
                    adapter.Fill(dt);

                    dgvMessages.DataSource = dt;

                    if (dgvMessages.Columns.Contains(IdColumn))
                    {
                        dgvMessages.Columns[IdColumn].HeaderText = "ID";
                        dgvMessages.Columns[IdColumn].FillWeight = 20;
                    }

                    if (dgvMessages.Columns.Contains(_messageColumnName))
                    {
                        dgvMessages.Columns[_messageColumnName].HeaderText = "留言";
                        dgvMessages.Columns[_messageColumnName].FillWeight = 80;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "読込失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectFirstRowIfAny()
        {
            if (dgvMessages.Rows.Count == 0)
            {
                return;
            }

            dgvMessages.ClearSelection();
            dgvMessages.Rows[0].Selected = true;
            dgvMessages.CurrentCell = dgvMessages.Rows[0].Cells[0];
        }

        private static void TestConnection()
        {
            using (var conn = new MySqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "SELECT 1;";
                cmd.ExecuteScalar();
            }
        }

        private void EnsureMessageColumnReady()
        {
            if (string.IsNullOrWhiteSpace(_messageColumnName))
            {
                _messageColumnName = DetectMessageColumnName();
            }
        }

        private static string DetectMessageColumnName()
        {
            using (var conn = new MySqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                
                cmd.CommandText =
                    "SELECT COUNT(*) FROM information_schema.COLUMNS " +
                    "WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @table AND COLUMN_NAME = 'tateno';";
                cmd.Parameters.AddWithValue("@table", TableName);

                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                {
                    return "tateno";
                }

               
                cmd.Parameters.Clear();
                cmd.CommandText =
                    "SELECT COLUMN_NAME " +
                    "FROM information_schema.COLUMNS " +
                    "WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @table " +
                    "  AND COLUMN_NAME <> @id " +
                    "  AND (DATA_TYPE LIKE '%char%' OR DATA_TYPE LIKE '%text%') " +
                    "ORDER BY ORDINAL_POSITION " +
                    "LIMIT 1;";
                cmd.Parameters.AddWithValue("@table", TableName);
                cmd.Parameters.AddWithValue("@id", IdColumn);

                var col = cmd.ExecuteScalar() as string;
                if (!string.IsNullOrWhiteSpace(col))
                {
                    return col;
                }

                throw new InvalidOperationException("留言用のカラムが見つかりません");
            }
        }

        private static bool ColumnExists(MySqlConnection conn, string tableName, string columnName)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT COUNT(*) " +
                    "FROM information_schema.COLUMNS " +
                    "WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @table AND COLUMN_NAME = @column;";
                cmd.Parameters.AddWithValue("@table", tableName);
                cmd.Parameters.AddWithValue("@column", columnName);

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
    }
}
