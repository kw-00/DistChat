
export type ElementWithLifecycle<TElement extends HTMLElement>
  = TElement & {
    destroy(): void;
  };

export class ComponentLifecycle {
  addLifecycle<TElement extends HTMLElement>(
    element: TElement, cleanupFn: () => void
  ): ElementWithLifecycle<TElement> {
    (element as ElementWithLifecycle<TElement>).destroy = cleanupFn;
    return element as ElementWithLifecycle<TElement>;
  }
}
