import Navigo from "navigo";
import { Root } from "@dt/root";
import { RegisterPage } from "@dt/auth/pages/register-page";
import { LoginPage } from "@dt/auth/pages/login-page";

const navigo = new Navigo("/");



export class Routing {
  static REGISTER = "register";
  static LOGIN = "login";

  static goTo(path: string): void {
    navigo.navigate(path);
  }
}

navigo.on("/", () => {
  navigo.navigate(Routing.LOGIN);
})

navigo.on(Routing.REGISTER, () => {
  Root.switchPage(RegisterPage());
});

navigo.on(Routing.LOGIN, () => {
  Root.switchPage(LoginPage());
});

navigo.resolve();
