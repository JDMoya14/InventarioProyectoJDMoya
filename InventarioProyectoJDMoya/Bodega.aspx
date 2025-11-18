<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Bodega.aspx.cs" Inherits="InventarioProyectoJDMoya.Bodega" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<link href="CSS/Estilo.css" rel="stylesheet" />
	<title>Bodega</title>
</head>
<body>
	<form id="form1" runat="server">
		<div>
			<h2>Catalogo de bodega</h2>
		</div>
		
		<div>
			<ul>
				<li><a class="active" href="#home">Inicio</a></li>
				<li><a href="TipoArticulos.aspx">Tipo Articulos</a></li>
				<li><a href="Bodega.aspx">Bodega</a></li>
				<li><a href="Articulos.aspx">Articulos</a></li>
			</ul>
		</div>

		<div>
			<label>Codigo: </label>
			<asp:TextBox ID="tcodigo" runat="server"></asp:TextBox>
			<label>Descripcion: </label>
			<asp:TextBox ID="tdescripcion" runat="server"></asp:TextBox>
			<label>Ubicacion: </label>
			<asp:TextBox ID="tubicacion" runat="server"></asp:TextBox>
		</div>

		<div>
			<asp:Button ID="bguardar" runat="server" Text="Guardar" OnClick="bguardar_Click"></asp:Button>
			<asp:Button ID="bmodificar" runat="server" Text="Modificar" OnClick="bmodificar_Click"></asp:Button>
			<asp:Button ID="beliminar" runat="server" Text="Eliminar" OnClick="beliminar_Click"></asp:Button>
		</div>
		<div>
	<asp:GridView ID="GridView1" runat="server" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" DataSourceID="SqlDataSource1"></asp:GridView>
			<asp:SqlDataSource ID="SqlDataSource1" runat="server"></asp:SqlDataSource>
</div>
	</form>
</body>
</html>
