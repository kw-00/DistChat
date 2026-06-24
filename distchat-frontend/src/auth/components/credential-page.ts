import { Routing } from "@dt/routing";

export function CredentialPage(
  type: "login" | "register"
): HTMLDivElement {
  const page = document.createElement("div");
  page.className = "page hp wp vs jb ac p-lg";

  const welcoming = document.createElement("h2");
  welcoming.innerText = type === "register" ?
    "Welcome!" : "Welcome back!";
      
  const form = document.createElement("form");
  form.className = "vs grow jc gap-sm";
  form.style.width = "30rem";

  form.innerHTML = `
    <label class="vs jb">
      Login
      <input name="login"/>
    </label>
    ${
      type === "register" ?
      `<label class="vs jb">
        Email
        <input type="email" name="email"/>
      </label>`
      : ""
    }
    <label class="vs jb">
      Password
      <input type="password" name="password"/>
    </label>
    <button class="primary" type="submit">
      ${type === "register" ? "Register account" : "Log in"}
    </button>
  `;

  const alternativeLink = document.createElement("a");
  if (type === "register") {
    alternativeLink.href = Routing.LOGIN;
    alternativeLink.innerText = "Log in instead";    
  } else {
    alternativeLink.href = Routing.REGISTER;
    alternativeLink.innerText = "Register an account";
  }
  page.append(
    welcoming, form, alternativeLink
  );
  return page;
}
