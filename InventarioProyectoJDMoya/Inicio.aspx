<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Inicio.aspx.cs" Inherits="InventarioProyectoJDMoya.Inicio" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<link href="CSS/Estilo.css" rel="stylesheet" />
	<title>Inicio</title>
</head>
<body>
	<form id="form1" runat="server">
		<div>
			<ul>
				<li><a class="active" href="#home">Inicio</a></li>
				<li><a href="TipoArticulos.aspx">Tipo Articulos</a></li>
				<li><a href="Bodega.aspx">Bodega</a></li>
				<li><a href="Articulos.aspx">Articulos</a></li>
			</ul>
		</div>
	</form>
</body>
</html>
