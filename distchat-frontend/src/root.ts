

const root = document.getElementById("root");
if (root === null) throw new Error("Root element is missing on start.");

const styling: Partial<CSSStyleProperties> = {
  height: "100svh",
  width: "100svw",
  display: "flex",
  flexDirection: "column",
  justifyContent: "space-between",
  alignItems: "stretch"
};

Object.assign(
  root,
  styling
);

export class Root {
  static switchPage(pageElement: HTMLElement): void {
    if (root === null) throw new Error("Root element is missing.");
    root.replaceChildren(pageElement);
  }
}
