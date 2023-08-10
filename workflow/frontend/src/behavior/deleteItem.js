export default function (G6) {
  G6.registerBehavior("deleteItem", {
    getEvents() {
      return {
        keydown: "onKeydown",
        mouseleave: "onCanvasLeave",
        mouseenter: "onCanvasFocus",
      };
    },
    onKeydown(e) {
      const items = this.graph.get("selectedItems");
      const focus = this.graph.get("focusGraphWrapper");
      if (e.keyCode === 46 && items && items.length > 0 && focus) {
        console.log(e.keyCode);
        if (this.graph.executeCommand) {
          this.graph.executeCommand("delete", {});
        } else {
          this.graph.remove(items[0]);
        }
      }
    },
    onCanvasLeave(e) {
      // console.log(2);
      this.graph.set("focusGraphWrapper", false);
    },
    onCanvasFocus() {
      // console.log("onCanvasFocus");
      this.graph.set("focusGraphWrapper", true);
    },
  });
}
