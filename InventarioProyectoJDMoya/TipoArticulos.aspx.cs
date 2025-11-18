using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace InventarioProyectoJDMoya
{
	public partial class TipoArticulos : System.Web.UI.Page
	{
		// Elige el nombre correcto de tu connectionString:
		// private string Cnx => ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
		private string Cnx => ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
				LlenarGrid();
		}

		private void LlenarGrid()
		{
			try
			{
				using (var con = new SqlConnection(Cnx))
				using (var da = new SqlDataAdapter("SELECT IdTipo, Nombre, Descripcion FROM TipoArticulo ORDER BY IdTipo DESC", con))
				{
					var dt = new DataTable();
					da.Fill(dt);
					GridView1.DataSource = dt;
					GridView1.EmptyDataText = "Sin registros de tipos de artículo.";
					GridView1.DataBind();
				}
			}
			catch (Exception ex)
			{
				Alert("Error al cargar tipos: " + ex.Message);
			}
		}

		private static void Alert(string message)
		{
			var page = (Page)HttpContext.Current.CurrentHandler;
			var safe = (message ?? "").Replace("'", "\\'");
			page.ClientScript.RegisterStartupScript(page.GetType(), "alert", $"alert('{safe}');", true);
		}

		protected void bguardar_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(tnombre.Text))
			{
				Alert("El Nombre es requerido.");
				return;
			}

			const string sql = @"INSERT INTO TipoArticulo (Nombre, Descripcion)
                                 VALUES (@Nombre, @Descripcion)";
			try
			{
				using (var con = new SqlConnection(Cnx))
				using (var cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@Nombre", tnombre.Text.Trim());
					cmd.Parameters.AddWithValue("@Descripcion", tdescripcion.Text.Trim());
					con.Open();
					cmd.ExecuteNonQuery();
				}
				LlenarGrid();
				Limpiar();
				Alert("Tipo de artículo guardado correctamente.");
			}
			catch (Exception ex)
			{
				Alert("Error al guardar tipo: " + ex.Message);
			}
		}

		protected void bmodificar_Click(object sender, EventArgs e)
		{
			if (!int.TryParse(tid.Text, out var id))
			{
				Alert("Seleccione un registro del grid para modificar.");
				return;
			}

			const string sql = @"UPDATE TipoArticulo
                                 SET Nombre=@Nombre, Descripcion=@Descripcion
                                 WHERE IdTipo=@Id";
			try
			{
				using (var con = new SqlConnection(Cnx))
				using (var cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@Nombre", tnombre.Text.Trim());
					cmd.Parameters.AddWithValue("@Descripcion", tdescripcion.Text.Trim());
					cmd.Parameters.AddWithValue("@Id", id);
					con.Open();
					cmd.ExecuteNonQuery();
				}
				LlenarGrid();
				Limpiar();
				Alert("Tipo de artículo modificado.");
			}
			catch (Exception ex)
			{
				Alert("Error al modificar tipo: " + ex.Message);
			}
		}

		protected void beliminar_Click(object sender, EventArgs e)
		{
			if (!int.TryParse(tid.Text, out var id))
			{
				Alert("Seleccione un registro del grid para eliminar.");
				return;
			}

			const string sql = @"DELETE FROM TipoArticulo WHERE IdTipo=@Id";
			try
			{
				using (var con = new SqlConnection(Cnx))
				using (var cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@Id", id);
					con.Open();
					cmd.ExecuteNonQuery();
				}
				LlenarGrid();
				Limpiar();
				Alert("Tipo de artículo eliminado.");
			}
			catch (Exception ex)
			{
				Alert("Error al eliminar tipo: " + ex.Message);
			}
		}

		protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			var row = GridView1.SelectedRow;
			if (row == null) return;

			// Asumiendo columnas: [Select] [IdTipo] [Nombre] [Descripcion]
			tid.Text = row.Cells.Count > 1 ? row.Cells[1].Text : "";
			tnombre.Text = row.Cells.Count > 2 ? row.Cells[2].Text : "";
			tdescripcion.Text = row.Cells.Count > 3 ? row.Cells[3].Text : "";
		}

		private void Limpiar()
		{
			tid.Text = "";
			tnombre.Text = "";
			tdescripcion.Text = "";
		}
	}
}
