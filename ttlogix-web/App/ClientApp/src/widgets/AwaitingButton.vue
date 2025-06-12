<script lang="ts" setup>
  import { computed, ref } from 'vue';
  import { useI18n } from 'vue-i18n';

  const props = withDefaults(
    defineProps<{ btnText: string, btnIcon: string, btnClass: string, enabled?: boolean }>(),
    {enabled: true}
  );
  const emit = defineEmits<{ (e: 'proceed', callback: () => void):void, (e: 'reenableButtons'): void}>();

  const { t } = useI18n({ useScope: 'global' });
  const buttonTxt = t(props.btnText);
  const buttonIcon = ref(props.btnIcon);
  const buttonClass = ref(props.btnClass);
  const btnDisabled = ref(false);

  const buttonFunction = async() => {
      btnDisabled.value = true;
      emit('proceed', () => { btnDisabled.value = false; emit('reenableButtons') })
  }
</script>

<template>
    <button :class="['btn','btn-sm','me-2',buttonClass]" :disabled="btnDisabled || !enabled" @click.prevent="buttonFunction">
        <div v-if="btnDisabled" class="spinner-border spinner-border-sm text-center me-1" role="status" style="padding-top: 2px; width:15px; height:15px !important;"></div>
        <i v-if="!btnDisabled" :class="[buttonIcon]"></i>
        {{ buttonTxt }}
    </button>
</template>
