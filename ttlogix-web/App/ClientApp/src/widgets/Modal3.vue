<script lang="ts" setup>
  import { store } from '@/store';
  import { Modal } from '@/store/commonModels/modal';
  import { watch, ref } from 'vue';

  const x = store.state.config.images.x;
  const props = defineProps<{ modalBase: Modal }>();
  const emit = defineEmits<{(e: 'close'): void }>();
  const mask = ref(true);

  const close = () => { 
    mask.value = false;
    setTimeout(() => {emit('close'); mask.value = true;}, 500);
  };

  const width = () => {
    return typeof props.modalBase.width == 'number' ? (props.modalBase.width + 'px') : props.modalBase.width;
  }

  const closeOnOutsideClick = () => {
    if(props.modalBase.closeonclick) close();
  }

  const cssClass = ref<string>();
  watch(() => props.modalBase.fnMod.type, (nVal,oVal) => {
    switch(props.modalBase.fnMod.type){
        case 'error':
          cssClass.value = 'text-white bg-danger'; break;
        case 'warning':
          cssClass.value = 'bg-warning'; break;
        case 'confirm':
          cssClass.value = 'bg-primary'; break;
      }
  });

  window.onkeydown = (ev: KeyboardEvent): any => {
    if(ev.key == 'Escape' && props.modalBase.closeonesc){
      close();
    }
  }
</script>

<template>
  <transition name="fade">
    <div class="mask d-flex align-items-center justify-content-center vh-100" v-if="mask && modalBase.on" @click.self="closeOnOutsideClick">
      <div class="modal-popup row modal-popup-in" :style="{ 'width': width() }">
        <div class="col-md-12">
          <div class="card" :class=cssClass>
            <div class="card-header"> 
              <div class="row">
                <div class="col-md-10">
                  <slot name="title"></slot>
                </div>
                <div class="col-md-2" v-if="modalBase.closeonx">
                  <div class="modal-popup-close float-end" @click="close()" :style="{'background-image': 'url(' + x + ')'}"></div>
                </div>
              </div>
            </div>
            <div class="card-body">
              <slot name="body"></slot>
            </div>
            <div class="card-footer" v-if="modalBase.showfooter">
              <slot name="footer"></slot>
            </div>
          </div>
        </div>
      </div>
    </div>
  </transition>
</template>
