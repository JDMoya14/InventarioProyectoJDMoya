using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace InventarioProyectoJDMoya
{
	public partial class Bodega : System.Web.UI.Page
	{
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
				using (var da = new SqlDataAdapter("SELECT Codigo, Descripcion, Ubicacion FROM Bodega ORDER BY Codigo", con))
				{
					var dt = new DataTable();
					da.Fill(dt);
					GridView1.DataSource = dt;
					GridView1.EmptyDataText = "Sin registros de bodega.";
					GridView1.DataBind();
				}
			}
			catch (Exception ex)
			{
				Alert("Error al cargar bodega: " + ex.Message);
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
			if (string.IsNullOrWhiteSpace(tcodigo.Text) || string.IsNullOrWhiteSpace(tdescripcion.Text))
			{
				Alert("Código y Descripción son requeridos.");
				return;
			}

			const string sql = @"INSERT INTO Bodega (Codigo, Descripcion, Ubicacion)
                                 VALUES (@Codigo, @Descripcion, @Ubicacion)";
			try
			{
				using (var con = new SqlConnection(Cnx))
				using (var cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@Codigo", tcodigo.Text.Trim());
					cmd.Parameters.AddWithValue("@Descripcion", tdescripcion.Text.Trim());
					cmd.Parameters.AddWithValue("@Ubicacion", tubicacion.Text.Trim());
					con.Open();
					cmd.ExecuteNonQuery();
				}
				LlenarGrid();
				Limpiar();
				Alert("Bodega guardada correctamente.");
			}
			catch (Exception ex)
			{
				Alert("Error al guardar bodega: " + ex.Message);
			}
		}

		protected void bmodificar_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(tcodigo.Text))
			{
				Alert("Ingrese el Código de la bodega a modificar.");
				return;
			}

			const string sql = @"UPDATE Bodega
                                 SET Descripcion=@Descripcion, Ubicacion=@Ubicacion
                                 WHERE Codigo=@Codigo";
			try
			{
				using (var con = new SqlConnection(Cnx))
				using (var cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@Codigo", tcodigo.Text.Trim());
					cmd.Parameters.AddWithValue("@Descripcion", tdescripcion.Text.Trim());
					cmd.Parameters.AddWithValue("@Ubicacion", tubicacion.Text.Trim());
					con.Open();
					var rows = cmd.ExecuteNonQuery();
					if (rows == 0) { Alert("No se encontró una bodega con ese Código."); return; }
				}
				LlenarGrid();
				Limpiar();
				Alert("Bodega modificada correctamente.");
			}
			catch (Exception ex)
			{
				Alert("Error al modificar bodega: " + ex.Message);
			}
		}

		protected void beliminar_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(tcodigo.Text))
			{
				Alert("Ingrese el Código de la bodega a eliminar.");
				return;
			}

			const string sql = @"DELETE FROM Bodega WHERE Codigo=@Codigo";
			try
			{
				using (var con = new SqlConnection(Cnx))
				using (var cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@Codigo", tcodigo.Text.Trim());
					con.Open();
					var rows = cmd.ExecuteNonQuery();
					if (rows == 0) { Alert("No se encontró una bodega con ese Código."); return; }
				}
				LlenarGrid();
				Limpiar();
				Alert("Bodega eliminada correctamente.");
			}
			catch (Exception ex)
			{
				Alert("Error al eliminar bodega: " + ex.Message);
			}
		}

		protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			var row = GridView1.SelectedRow;
			if (row == null) return;

			//  columnas: [Select] [Codigo] [Descripcion] [Ubicacion]
			tcodigo.Text = row.Cells.Count > 1 ? row.Cells[1].Text : "";
			tdescripcion.Text = row.Cells.Count > 2 ? row.Cells[2].Text : "";
			tubicacion.Text = row.Cells.Count > 3 ? row.Cells[3].Text : "";
		}

		private void Limpiar()
		{
			tcodigo.Text = "";
			tdescripcion.Text = "";
			tubicacion.Text = "";
		}
	}
}