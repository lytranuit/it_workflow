<template>
  <div :data-clazz="model.clazz">
    <div class="panelTitle">{{ i18n["mail"] }}</div>
    <div class="panelBody">
      <Accordion :activeIndex="0">
        <AccordionTab :header="i18n['detail.general']">
          <DefaultDetail :model="model" :onChange="onChange" />
          <SettingMail :model="model"></SettingMail>
        </AccordionTab>
      </Accordion>
    </div>
  </div>
</template>
<script>
import DefaultDetail from "./DefaultDetail.vue";
import SettingMail from "./SettingMail.vue";
export default {
  inject: ["i18n"],
  components: {
    SettingMail,
    DefaultDetail,
  },
  data() {
    return {
      activeName: "1",
    };
  },

  props: {
    model: {
      type: Object,
      default: () => ({}),
    },
    onChange: {
      type: Function,
      default: () => {},
    },
  },
  mounted() {},
  methods: {},
};
</script>
<style></style>
