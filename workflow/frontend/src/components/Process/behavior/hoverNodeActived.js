export default function (G6) {
  G6.registerBehavior("hoverNodeActived", {
    getEvents() {
      return {
        "node:mouseenter": "onNodeEnter",
        "node:mouseleave": "onNodeLeave",
        "anchor:mouseleave": "onAnchorLeave",
      };
    },
    onAnchorLeave(e) {
      let node = e.item.getContainer().getParent();
      if (node && !this.graph.get("edgeDragging")) {
        this.graph.setItemState(node.get("item"), "show-anchor", false);
      }
    },
    onNodeEnter(e) {
      const clazz = e.item.getModel().clazz;
      if (clazz !== "endEvent" && !this.graph.get("edgeDragging")) {
        this.graph.setItemState(e.item, "show-anchor", true);
      }
    },
    onNodeLeave(e) {
      if (
        !(e.target.get("name") == "anchor-shape") &&
        !this.graph.get("edgeDragging")
      ) {
        this.graph.setItemState(e.item, "show-anchor", false);
      }
    },
  });
}
