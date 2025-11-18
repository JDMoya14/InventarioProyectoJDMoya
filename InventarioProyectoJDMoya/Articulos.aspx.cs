using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace InventarioProyectoJDMoya
{
	public partial class Articulos : System.Web.UI.Page
	{
		// Usa el nombre correcto de tu connectionString:
		// private string Cnx => ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
		private string Cnx => ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				CargarCombos();  // similar a "LlenarTipos" del repo de apoyo
				LlenarGrid();
			}
		}

		private void CargarCombos()
		{
			try
			{
				using (var con = new SqlConnection(Cnx))
				{
					// Tipos
					using (var da = new SqlDataAdapter("SELECT IdTipo, Nombre FROM TipoArticulo ORDER BY Nombre", con))
					{
						var dt = new DataTable();
						da.Fill(dt);
						ddTipo.DataSource = dt;
						ddTipo.DataValueField = "IdTipo";
						ddTipo.DataTextField = "Nombre";
						ddTipo.DataBind();
					}

					// Bodegas
					using (var da = new SqlDataAdapter("SELECT IdBodega, Descripcion FROM Bodega ORDER BY Descripcion", con))
					{
						var dt = new DataTable();
						da.Fill(dt);
						ddBodega.DataSource = dt;
						ddBodega.DataValueField = "IdBodega";
						ddBodega.DataTextField = "Descripcion";
						ddBodega.DataBind();
					}
				}
			}
			catch (Exception ex)
			{
				Alert("Error al cargar combos: " + ex.Message);
			}
		}

		private void LlenarGrid()
		{
			const string sql = @"
                SELECT a.Codigo, a.Nombre, t.Nombre AS Tipo, b.Descripcion AS Bodega, a.Cantidad, a.Costo
                FROM Articulo a
                INNER JOIN TipoArticulo t ON t.IdTipo = a.IdTipo
                INNER JOIN Bodega b ON b.IdBodega = a.IdBodega
                ORDER BY a.Codigo";
			try
			{
				using (var con = new SqlConnection(Cnx))
				using (var da = new SqlDataAdapter(sql, con))
				{
					var dt = new DataTable();
					da.Fill(dt);
					GridView1.DataSource = dt;
					GridView1.EmptyDataText = "Sin registros de artículos.";
					GridView1.DataBind();
				}
			}
			catch (Exception ex)
			{
				Alert("Error al cargar artículos: " + ex.Message);
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
			if (string.IsNullOrWhiteSpace(tcodigo.Text) || string.IsNullOrWhiteSpace(tnombre.Text))
			{
				Alert("Código y Nombre son requeridos.");
				return;
			}
			if (!int.TryParse(tcantidad.Text, out var cantidad)) cantidad = 0;
			if (!decimal.TryParse(tcosto.Text, out var costo)) costo = 0m;

			const string sql = @"
                INSERT INTO Articulo (Codigo, Nombre, IdTipo, IdBodega, Cantidad, Costo)
                VALUES (@Codigo, @Nombre, @IdTipo, @IdBodega, @Cantidad, @Costo)";
			try
			{
				using (var con = new SqlConnection(Cnx))
				using (var cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@Codigo", tcodigo.Text.Trim());
					cmd.Parameters.AddWithValue("@Nombre", tnombre.Text.Trim());
					cmd.Parameters.AddWithValue("@IdTipo", ddTipo.SelectedValue);
					cmd.Parameters.AddWithValue("@IdBodega", ddBodega.SelectedValue);
					cmd.Parameters.AddWithValue("@Cantidad", cantidad);
					cmd.Parameters.AddWithValue("@Costo", costo);
					con.Open();
					cmd.ExecuteNonQuery();
				}
				LlenarGrid();
				Limpiar();
				Alert("Artículo guardado correctamente.");
			}
			catch (Exception ex)
			{
				Alert("Error al guardar artículo: " + ex.Message);
			}
		}

		protected void bmodificar_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(tcodigo.Text))
			{
				Alert("Ingrese el Código del artículo a modificar o selecciónelo del grid.");
				return;
			}
			if (!int.TryParse(tcantidad.Text, out var cantidad)) cantidad = 0;
			if (!decimal.TryParse(tcosto.Text, out var costo)) costo = 0m;

			const string sql = @"
                UPDATE Articulo
                SET Nombre=@Nombre, IdTipo=@IdTipo, IdBodega=@IdBodega, Cantidad=@Cantidad, Costo=@Costo
                WHERE Codigo=@Codigo";
			try
			{
				using (var con = new SqlConnection(Cnx))
				using (var cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@Nombre", tnombre.Text.Trim());
					cmd.Parameters.AddWithValue("@IdTipo", ddTipo.SelectedValue);
					cmd.Parameters.AddWithValue("@IdBodega", ddBodega.SelectedValue);
					cmd.Parameters.AddWithValue("@Cantidad", cantidad);
					cmd.Parameters.AddWithValue("@Costo", costo);
					cmd.Parameters.AddWithValue("@Codigo", tcodigo.Text.Trim());
					con.Open();
					var rows = cmd.ExecuteNonQuery();
					if (rows == 0) { Alert("No se encontró un artículo con ese Código."); return; }
				}
				LlenarGrid();
				Limpiar();
				Alert("Artículo modificado correctamente.");
			}
			catch (Exception ex)
			{
				Alert("Error al modificar artículo: " + ex.Message);
			}
		}

		protected void beliminar_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(tcodigo.Text))
			{
				Alert("Ingrese el Código del artículo a eliminar o selecciónelo del grid.");
				return;
			}

			const string sql = @"DELETE FROM Articulo WHERE Codigo=@Codigo";
			try
			{
				using (var con = new SqlConnection(Cnx))
				using (var cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@Codigo", tcodigo.Text.Trim());
					con.Open();
					var rows = cmd.ExecuteNonQuery();
					if (rows == 0) { Alert("No se encontró un artículo con ese Código."); return; }
				}
				LlenarGrid();
				Limpiar();
				Alert("Artículo eliminado correctamente.");
			}
			catch (Exception ex)
			{
				Alert("Error al eliminar artículo: " + ex.Message);
			}
		}

		protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			var row = GridView1.SelectedRow;
			if (row == null) return;

			// Asumiendo columnas: [Select] [Codigo] [Nombre] [Tipo] [Bodega] [Cantidad] [Costo]
			tcodigo.Text = row.Cells.Count > 1 ? row.Cells[1].Text : "";
			tnombre.Text = row.Cells.Count > 2 ? row.Cells[2].Text : "";

			// Traigo el registro real para setear los combos y numéricos
			try
			{
				using (var con = new SqlConnection(Cnx))
				using (var da = new SqlDataAdapter("SELECT * FROM Articulo WHERE Codigo=@Codigo", con))
				{
					da.SelectCommand.Parameters.AddWithValue("@Codigo", tcodigo.Text);
					var dt = new DataTable();
					da.Fill(dt);
					if (dt.Rows.Count > 0)
					{
						var r = dt.Rows[0];
						ddTipo.SelectedValue = r["IdTipo"].ToString();
						ddBodega.SelectedValue = r["IdBodega"].ToString();
						tcantidad.Text = r["Cantidad"].ToString();
						tcosto.Text = Convert.ToDecimal(r["Costo"]).ToString("0.##");
					}
				}
			}
			catch (Exception ex)
			{
				Alert("Error al leer detalle del artículo: " + ex.Message);
			}
		}

		private void Limpiar()
		{
			tcodigo.Text = "";
			tnombre.Text = "";
			tcantidad.Text = "";
			tcosto.Text = "";
			if (ddTipo.Items.Count > 0) ddTipo.SelectedIndex = 0;
			if (ddBodega.Items.Count > 0) ddBodega.SelectedIndex = 0;
		}
	}
}