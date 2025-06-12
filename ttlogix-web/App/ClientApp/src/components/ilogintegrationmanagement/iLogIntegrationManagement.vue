<script lang="ts" setup>
    // import vue stuff
    import { ref } from 'vue';
    import { useI18n } from 'vue-i18n';

    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { CsvFile } from '@/store/iLogIntegration/models/csvFile';

    // import widgets and modals
    import ConfirmPopup from '@/widgets/ConfirmPopup.vue';

    // import common logic
    import * as iLogIntegrationActions from '@/store/iLogIntegration/iLogIntegrationActions'

    // vue and global vars
    const { t } = useI18n({ useScope: 'global' });

    // local vars
    const isEnabled = ref(false);
    const loading = ref(true);

    // logic
    const getIlogState = () => {
        loading.value = true;
        iLogIntegrationActions.isEnabled().then((state: boolean) => { isEnabled.value = state; loading.value = false; });
        
    };
    const getStockFile = () => {
        iLogIntegrationActions.getStockFile().then((file: CsvFile) => {
            const blob = new Blob([file.csv], { type: 'application/csv' });
            const link = document.createElement('a');
            link.href = URL.createObjectURL(blob);
            link.download = file.name;
            link.click();
            URL.revokeObjectURL(link.href);
        });
    }
    const toggleIntegrationState = (enable: boolean) => {
        var message = '';
        if (enable) {
            message = '<span class="main-action-icons-small"><i class="las la-exclamation-triangle"></i></span> <strong>';
            message += t('iLogIntegration.warningEnable');
            message += '</strong><br /> <br />';
            message += t('iLogIntegration.questionEnable');
            confirmModal.value.fnMod.callback = async () => { await enableExecute() };
        }
        else {
            message = t('iLogIntegration.questionDisable');
            confirmModal.value.fnMod.callback = async () => { await disableExecute() };
        }
        confirmModal.value.fnMod.message = message;
        confirmModal.value.on = true;
    };
    const enableExecute = () => {
        iLogIntegrationActions.enable().then(() => {
            getIlogState();
            closeModal(confirmModal.value);
        });
    };
    const disableExecute = () => {
        iLogIntegrationActions.disable().then(() => {
            getIlogState();
            closeModal(confirmModal.value);
        });
    };

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const confirmModal = ref(new Modal(600, 'Please confirm', false));

    // init the screen
    getIlogState();
</script>
<template>
    <div class="container mt-4">
        <div class="row">
            <div class="col-md-12">
                <h3>{{ t('iLogIntegration.header') }}</h3>
            </div>
        </div>
        <div class="row mt-4">
            <div class="col-md-6">
                <div class="card border-dark">
                    <div class="card-header">
                        <strong>{{ t('iLogIntegration.stockFileGeneration') }}</strong>
                    </div>
                    <div class="card-body">
                        <div class="text-center my-5">
                            <button class="btn btn btn-primary" :disabled="isEnabled" @click="getStockFile">{{ t('iLogIntegration.getStockFile') }}</button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="card border-dark">
                    <div class="card-header">
                        <strong>{{ t('iLogIntegration.intergationSwith') }}</strong>
                    </div>
                    <div class="card-body">
                        <div v-if="!loading">
                            <div class="card border-danger mb-3" v-if="!isEnabled">
                                <div class="card-body bg-warning">
                                    <strong>{{ t('iLogIntegration.instructionTitle') }}</strong>
                                    <ol>
                                        <li>{{ t('iLogIntegration.instructionP1') }}</li>
                                        <li>{{ t('iLogIntegration.instructionP2') }}</li>
                                        <li>{{ t('iLogIntegration.instructionP3') }}</li>
                                        <li v-html="t('iLogIntegration.instructionP4')"></li>
                                        <li v-html="t('iLogIntegration.instructionP5')"></li>
                                    </ol>
                                </div>
                            </div>
                            <div class="text-center mt-1">
                                <strong :class="isEnabled ? 'ilog-enabled-txt' : 'ilog-disabled-txt'">{{ isEnabled ? t('iLogIntegration.enabled') : t('iLogIntegration.disabled') }}</strong>
                            </div>
                            <div class="text-center">
                                <span class="indicator" :class="isEnabled ? 'ilog-enabled' : 'ilog-disabled'">&nbsp;</span>
                            </div>
                            <div class="text-center mt-3">
                                <button class="btn btn btn-danger me-4" @click="toggleIntegrationState(false)" :disabled="!isEnabled"><i class="las la-ban"></i> {{ t('iLogIntegration.disable') }}</button>
                                <button class="btn btn btn-success" @click="toggleIntegrationState(true)" :disabled="isEnabled"><i class="las la-power-off"></i> {{ t('iLogIntegration.enable') }}</button>
                            </div>
                        </div>
                        <div v-if="loading" class="bars1-wrapper mt-4 mb-5">
                            <div id="bars1">
                                <span></span>
                                <span></span>
                                <span></span>
                                <span></span>
                                <span></span>
                            </div>
                            <h5>Loading...</h5>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <confirm-popup :modalBase=confirmModal @close="closeModal(confirmModal); getIlogState()" />
</template> 