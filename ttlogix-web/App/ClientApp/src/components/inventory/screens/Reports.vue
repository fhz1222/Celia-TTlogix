<script lang="ts" setup>
    // import vue stuff
    import { ref } from 'vue';
    import { store } from '@/store';
    import { ActionTypes } from '@/store/action-types';

    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { Report } from '@/store/commonModels/report';
    import { ReportFilter } from '@/store/commonModels/reportFilter';

    // import widgets and modals
    import ReportPopup from '../modals/Report.vue';
    import PDFPreviewer from '@/widgets/PDFPreviewer.vue';
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import IntervalPopup from '../modals/Interval.vue';
    import LocationSummary from '../modals/LocationSummary.vue';
    import Dates from '../modals/Dates.vue';
    
    // variables
    const rb = 'report/inventory/'; // report base path

    // functions
    const report = (report:Report) => {
        if(report.popup){
            report.popup.on = true;
            report.popup.customProps = { ...report.props, report: report };
        } else {
            printReport(null, null, report);
        }
    };
    const printReport = async (modalToClose: Modal | null = null, filter:ReportFilter | null = null, report: Report) => {
        if(modalToClose) closeModal(modalToClose);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.INV_REPORT, { filter, report })
                   .then((report) => {
                       waitModal.value.on = false;
                       pdfPreviewer.value.customProps = { file: report, name: report.name };
                       pdfPreviewer.value.on = true;
                   })
                   .catch((err:Error) => {
                        closeModal(waitModal.value);
                        errorModal.value.on = true;
                        errorModal.value.customProps = err;
                        errorModal.value.fnMod.type = 'error';
                    });
    };

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const rep1 = ref(new Modal(650, 'Report filter', true));
    const rep2 = ref(new Modal(650, 'Report filter', true));
    const rep3 = ref(new Modal(650, 'Report filter', true));
    const rep4 = ref(new Modal(650, 'Report filter', true));
    const waitModal = ref(new Modal(500,'Printing', false));
    const pdfPreviewer = ref(new Modal('90%','Report',true));
    const errorModal = ref(new Modal(500, 'Error', false));

    // reports
    const reportProps = {
        inventory:                    { showBonded:false, showDates:false, reqDates:false, showProducts:false, showRepBy:false, showSupp:false, reqSupp:false },
        inventorymovement:            { showBonded:true,  showDates:true,  reqDates:true,  showProducts:true,  showRepBy:false, showSupp:true,  reqSupp:true },
        inventorymovementsummary:     { showBonded:true,  showDates:true,  reqDates:true,  showProducts:true,  showRepBy:false, showSupp:true,  reqSupp:true },
        inventorytransaction:         { showBonded:true,  showDates:true,  reqDates:true,  showProducts:true,  showRepBy:false, showSupp:true,  reqSupp:true },
        m3detailed:                   { showBonded:true,  showDates:false, reqDates:false, showProducts:true,  showRepBy:false, showSupp:true,  reqSupp:true },
        m3summary:                    { showBonded:true,  showDates:false, reqDates:false, showProducts:true,  showRepBy:false, showSupp:true,  reqSupp:false },
        inventorylocation:            { showBonded:false, showDates:false, reqDates:false, showProducts:true,  showRepBy:false, showSupp:false, reqSupp:false },
        locationdetail:               { showBonded:false, showDates:false, reqDates:false, showProducts:false, showRepBy:false, showSupp:false, reqSupp:false },
        aging:                        {},
        partsmasterlist:              { showBonded:true,  showDates:false, reqDates:false, showProducts:true,  showRepBy:false, showSupp:true,  reqSupp:false },
        agingbycontrolcode:           { showBonded:false, showDates:true,  reqDates:true,  showProducts:true,  showRepBy:false, showSupp:false, reqSupp:false },
        spq:                          { showBonded:false, showDates:false, reqDates:false, showProducts:false, showRepBy:false, showSupp:false, reqSupp:false },
        inventorywithstandbylocation: { showBonded:false, showDates:false, reqDates:false, showProducts:false, showRepBy:false, showSupp:false, reqSupp:false },
        locationsummary:              {},
        pricetracking:                { showBonded:true,  showDates:false, reqDates:false, showProducts:false, showRepBy:false, showSupp:true,  reqSupp:false },
        // VMI reports
        inboundsummary:               { bothDates: true },
        outboundsummary:              { bothDates: true },
        inventoryasof:                { bothDates: false },
        transaction:                  { bothDates: true }
    };
    const reports:Array<Report> = [
        {title:'Inventory',                 alias:rb+'inventory',                   popup:rep1.value, props: reportProps.inventory }, // popup1 - CUSTOMER ONLY
        {title:'Inventory Movement',        alias:rb+'inventorymovement',           popup:rep1.value, props: reportProps.inventorymovement }, // popup 1 + bonded + from-to dates
        {title:'Inventory Movement Summary',alias:rb+'inventorymovementsummary',    popup:rep1.value, props: reportProps.inventorymovementsummary }, // popup 1 + bonded + from-to dates
        {title:'Inventory Transaction',     alias:rb+'inventorytransaction',        popup:rep1.value, props: reportProps.inventorytransaction }, // popup 1 + bonded + from-to dates
        {title:'M3 (Detailed)',             alias:rb+'m3detailed',                  popup:rep1.value, props: reportProps.m3detailed }, // popup 1 + bonded
        {title:'M3 (Summary)',              alias:rb+'m3summary',                   popup:rep1.value, props: reportProps.m3summary }, // popup 1 + bonded
        {title:'Inventory By Location',     alias:rb+'inventorylocation',           popup:rep1.value, props: reportProps.inventorylocation }, // popup 1 ONLY PRODUCTS
        {title:'Location Details',          alias:rb+'locationdetail',              popup:rep1.value, props: reportProps.locationdetail }, 
        {title:'Aging',                     alias:rb+'aging',                       popup:rep2.value, props: null }, // popup 2 - only day interval
        {title:'Part Master',               alias:rb+'partsmasterlist',             popup:rep1.value, props: reportProps.partsmasterlist }, // popup 1 + bonded
        {title:'Aging By Control Code',     alias:rb+'agingbycontrolcode',          popup:rep1.value, props: reportProps.agingbycontrolcode }, // popup 1 ONLY PRODUCTS + from-to dates
        {title:'SPQ',                       alias:rb+'spq',                         popup:rep1.value, props: reportProps.spq },
        {title:'Stand By Inventory',        alias:rb+'inventorywithstandbylocation',popup:rep1.value, props: reportProps.inventorywithstandbylocation },
        {title:'Location Summary',          alias:rb+'locationsummary',             popup:rep3.value, props: null }, // popup 3
        {title:'Price Tracking',            alias:rb+'pricetracking',               popup:rep1.value, props: reportProps.pricetracking }  // popup 1 ONLY SUPPLIER + bonded
    ];
    const vmiReports:Array<Report> = [
        {title:'Inbound Summary', alias:rb+'inboundsummary', popup:rep4.value, props: reportProps.inboundsummary }, // p1 dates only
        {title:'Outbound Summary',alias:rb+'outboundsummary',popup:rep4.value, props: reportProps.outboundsummary }, // p1 dates only
        {title:'Inventory',       alias:rb+'inventoryasof',  popup:rep4.value, props: reportProps.inventoryasof }, // p1 dates - FROM ONLY
        {title:'Transaction',     alias:rb+'transaction',    popup:rep4.value, props: reportProps.transaction }, // p1 dates only
    ];
</script>
    
<template>
    <div class="row mt-4 mx-2">
        <div class="col-md-12">
            <div class="card">
                <div class="card-header">
                    <h5>Reports</h5>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-3" :class="i > 3 ? 'mt-4' : ''" v-for="rep, i in reports" :key="i">
                            <button class="btn btn-primary" @click="report(rep)">{{i+1}}. {{rep.title}}</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="row mt-4 mx-2">
        <div class="col-md-12">
            <div class="card">
                <div class="card-header">
                    <h5>VMI Reports</h5>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-3" :class="i > 3 ? 'mt-4' : ''" v-for="rep, i in vmiReports" :key="i">
                            <button class="btn btn-primary" @click="report(rep)">{{i+1}}. {{rep.title}}</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <report-popup :modalBase=rep1 @close="closeModal(rep1)" @print="printReport" />
    <interval-popup :modalBase=rep2 @close="closeModal(rep2)" @print="printReport" />
    <location-summary :modalBase=rep3 @close="closeModal(rep3)" @print="printReport" />
    <dates :modalBase=rep4 @close="closeModal(rep4)" @print="printReport" />
    <p-d-f-previewer :modalBase=pdfPreviewer @close="closeModal(pdfPreviewer)" />
    <processing-popup :modalBase=waitModal />
    <error-popup :modalBase=errorModal @close="closeModal(errorModal)" />
</template>
