Imports System
Partial Class HeaderPageMaster
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        lblName.Text = Session("full_name")
        CheckLogin()
    End Sub

    Protected Sub btnLogOut_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogOut.Click
        HttpContext.Current.Session.Abandon()
        Response.Redirect("Login.aspx")
    End Sub
    Private Sub CheckLogin()
        If Me.Session("uname") = "" Then
            Response.Redirect("Login.aspx")
        End If
    End Sub
End Class

