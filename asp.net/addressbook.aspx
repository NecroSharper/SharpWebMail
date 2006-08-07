<%@ Page Language="C#"  Inherits="anmar.SharpWebMail.UI.AddressBook" trace="false" validateRequest="true" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
	<title><%=Application["product"]%></title>
	<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
	<link rel="stylesheet" type="text/css" href="sharpwebmail.css">
</head>
<body bgcolor="#FFFFFF" text="#000000" leftmargin="0" topmargin="0" marginwidth="0" marginheight="0">
<script type="text/javascript"><!--
function add_rcpt (email) {
	var dest = window.opener.document.sharpwebmailform.elements['SharpUI:toemail'];
	if ( typeof (dest)=='undefined' )
		dest = window.opener.document.sharpwebmailform.elements['SharpUI$toemail'];
	if ( dest!=null ) {
		dest.value = dest.value.replace(/(?:^\s+|\s+$|;$)/, '');
		var current = dest.value.split(',');
		for ( var i=0; current!=null&&i<current.length; i++ ) {
			current[i] = current[i].replace(/(?:^\s+|\s+$)/, '');
			if ( current[i]==email )
				return;
		}
		if ( dest.value.length>0 )
			dest.value += ', ';
		dest.value += email;
	}
}
//--></script>
	<form id="sharpwebmailform" method="post" runat="server">
		<select ID="addressbookselect" OnChange="this.form.submit();" OnServerChange="AddressBook_Changed" Visible="false" runat="server" />
		<asp:DataGrid id="AddressBookDataGrid" runat="server"
			AllowPaging="True"
			PageSize="10"
			PagerStyle-HorizontalAlign="Right"
			OnPageIndexChanged="AddressBookDataGrid_PageIndexChanged"
			BorderColor="black"
			BorderWidth="1"
			GridLines="Both"
			CellPadding="3"
			CellSpacing="0"
			EnableViewState="true"
			HeaderStyle-CssClass="XPPanel"
			AutoGenerateColumns="false">
			<Columns>
				<asp:TemplateColumn>
					<HeaderTemplate>
						<asp:Label id="inboxHeaderNumber" Text="<%# this.AddressBookDataGrid.Columns[0].HeaderText %>" runat="server" />
					</HeaderTemplate>
					<ItemTemplate>
						<a href="javascript:add_rcpt('<%#((System.Data.DataRowView)Container.DataItem)[1].ToString()%>')">
						<asp:Label runat="server"
							Text='<%# System.Web.HttpUtility.HtmlEncode (((System.Data.DataRowView)Container.DataItem)[0].ToString()) %>' />
							-
							<asp:Label runat="server"
							Text='<%# System.Web.HttpUtility.HtmlEncode (((System.Data.DataRowView)Container.DataItem)[1].ToString()) %>' />
						</a>
					</ItemTemplate>
				</asp:TemplateColumn>
			</Columns>
		</asp:DataGrid>
	</form>
</body>
</html>
