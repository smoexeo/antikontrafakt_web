<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="Антиконтрафакт2._1.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width" />
    <title></title>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css" />
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js" ></script>
    <script src="https://cdn.jsdelivr.net/npm/popper.js@1.16.0/dist/umd/popper.min.js" ></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/js/bootstrap.min.js"></script>
</head>
<body>
    <header>
        <nav class="navbar navbar-light bg-light justify-content-between">
            <asp:HyperLink runat="server" NavigateUrl="~/login.aspx" class="navbar-brand" style="font-family: Roboto; font-style: normal; font-weight: normal; font-size: 32px; line-height: 49px; display: flex; align-items: center; text-align: center; color: #F3533B;">АНТИКонтрафакт</asp:HyperLink>
            <form class="form-inline my-auto justify-content-between">

                <span style="margin-right: 10px;">
                </span>

                <div class="justify-content-center">
                    <asp:HyperLink runat="server" NavigateUrl="~/login.aspx">
                        <img src="person-outline.svg" width="44" />
                    </asp:HyperLink>

                </div>
            </form>
        </nav>


    </header>


    <div>

        <div class="container " style="background-color: #F3533B; padding-top: 40px; padding-bottom: 40px; width: 400px">
            <div class="row justify-content-center">
                <div class=" col-xs-4">

                    <div class="form-group">
                        <label for="exampleInputEmail1">Почта</label>
                        <input type="email" class="form-control" id="exampleInputEmail1" aria-describedby="emailHelp" placeholder="Enter email" />
                    </div>

                    <div class="form-group">
                        <label for="exampleInputPassword1">Пароль</label>
                        <input type="password" class="form-control" id="exampleInputPassword1" placeholder="Password" />
                    </div>

                    <button type="submit" class="btn btn-primary">Войти</button>
                    <button class="btn btn-primary">Забыли пароль?</button>
                    <button class="btn btn-primary">Регистрация</button>

                </div>
            </div>
        </div>
    </div>
    <footer>
    </footer>

</body>

</html>
