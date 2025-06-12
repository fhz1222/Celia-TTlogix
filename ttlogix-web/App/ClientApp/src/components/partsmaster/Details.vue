<template>
    <modal containerClass="modal-partsmaster-details">
        <template name="header" v-slot:header>
            {{title}}
        </template>
        <template name="body" v-slot:body>

            <div class="tab-content" id="myTabContent">
                <div class="tab-pane fade show active" id="properties" role="tabpanel" aria-labelledby="properties-tab">

                    <div class="row">
                        <div class="col-md-5">
                            <!-- Properties -->
                            <!--<div class="ps-3 pt-3 pb-3">-->
                                <div><strong>{{$t('partsMaster.details.properties')}}</strong></div>
                                <div class="card">
                                    <div class="card-body">
                                        <!-- Part No -->
                                        <div class="row" v-if="productCodeMap.pC1Name">
                                            <div class="col-md-3 pt-1 text-end no-right-padding">
                                                {{productCodeMap.pC1Name}}
                                            </div>
                                            <div class="col-md-9">
                                                <input type="text" v-model="model.productCode1" class="form-control form-control-sm" :disabled="readonly || type != 'new'" />
                                                <form-error :errors="errors" field="ProductCode1" />
                                            </div>
                                        </div>
                                        <!-- Supplier Part No -->
                                        <div class="row mt-2" v-if="productCodeMap.pC2Name">
                                            <div class="col-md-3 pt-1 text-end no-right-padding">
                                                {{productCodeMap.pC2Name}}
                                            </div>
                                            <div class="col-md-9">
                                                <input type="text" v-model="model.productCode2" class="form-control form-control-sm" :disabled="readonly" />
                                                <form-error :errors="errors" field="ProductCode2" />
                                            </div>
                                        </div>
                                        <!-- ProductCode3 -->
                                        <div class="row mt-2" v-if="productCodeMap.pC3Name">
                                            <div class="col-md-3 pt-1 text-end no-right-padding">
                                                {{productCodeMap.pC3Name}}
                                            </div>
                                            <div class="col-md-9">
                                                <input type="text" v-model="model.productCode3" class="form-control form-control-sm" :disabled="readonly" />
                                                <form-error :errors="errors" field="ProductCode3" />
                                            </div>
                                        </div>
                                        <!-- ProductCode4 -->
                                        <div class="row mt-2" v-if="productCodeMap.pC4Name">
                                            <div class="col-md-3 pt-1 text-end no-right-padding">
                                                {{productCodeMap.pC4Name}}
                                            </div>
                                            <div class="col-md-9">
                                                <input type="text" v-model="model.productCode4" class="form-control form-control-sm" :disabled="readonly" />
                                                <form-error :errors="errors" field="ProductCode4" />
                                            </div>
                                        </div>
                                        <!-- Description -->
                                        <div class="row mt-2">
                                            <div class="col-md-3 pt-1 text-end no-right-padding">
                                                {{$t('partsMaster.details.description')}}
                                            </div>
                                            <div class="col-md-9">
                                                <textarea type="text" v-model="model.description" class="form-control form-control-sm" :disabled="readonly"></textarea>
                                                <form-error :errors="errors" field="Description" />
                                            </div>
                                        </div>
                                        <!-- Unit of measure -->
                                        <div class="row mt-2">
                                            <div class="col-md-3 pt-1 text-end no-right-padding">
                                                {{$t('partsMaster.details.uom')}}
                                            </div>
                                            <div class="col-md-3 multiselect-as-select">
                                                <select-uom v-model="model.uom" :customerCode="customerCode" :disabled="readonly"></select-uom>
                                                <form-error :errors="errors" field="UOM" />
                                            </div>
                                            <!-- Status -->
                                            <div class="col-md-3 pt-1 text-end no-right-padding">
                                                {{$t('partsMaster.details.status')}}
                                            </div>
                                            <div class="col-md-3 multiselect-as-select">
                                                <multiselect v-model="model.status"
                                                             :options="Object.keys(statuses).map(x => parseInt(x))"
                                                             :close-on-select="true"
                                                             :searchable="false"
                                                             placeholder="select"
                                                             select-label=""
                                                             deselect-label=""
                                                             selected-label=""
                                                             :disabled="readonly"
                                                             :can-clear="false"
                                                             :can-deselect="false"
                                                             :classes="{container: 'form-control-sm multiselect'}">
                                                </multiselect>
                                                <form-error :errors="errors" field="Status" />
                                            </div>
                                        </div>
                                        <!-- Country of Origin -->
                                        <div class="row mt-2">
                                            <div class="col-md-3 pt-1 text-end no-right-padding">
                                                {{$t('partsMaster.details.origin')}}
                                            </div>
                                            <div class="col-md-9 multiselect-as-select">
                                                <select-country v-model="model.originCountry" :disabled="readonly"></select-country>
                                                <form-error :errors="errors" field="OriginCountry" />
                                            </div>
                                        </div>

                                        <!-- Serial -->
                                        <div class="row mt-2">
                                            <div class="col-md-3 pt-1 text-end no-right-padding">
                                                {{$t('partsMaster.details.serial')}}
                                            </div>
                                            <div class="col-md-9">
                                                <div class="row">
                                                    <div class="col-md-3">
                                                        <div class="form-check pt-1">
                                                            <input class="form-check-input" type="radio" v-model="model.enableSerialNo" v-bind:value="true" name="serial" id="serial1" :disabled="readonly">
                                                            <label class="form-check-label" for="serial1">
                                                                {{$t('YES')}}
                                                            </label>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3">
                                                        <div class="form-check pt-1">
                                                            <input class="form-check-input" type="radio" v-model="model.enableSerialNo" v-bind:value="false" name="serial" id="serial2" :disabled="readonly">
                                                            <label class="form-check-label" for="serial2">
                                                                {{$t('NO')}}
                                                            </label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <form-error :errors="errors" field="EnableSerialNo" />
                                            </div>
                                        </div>
                                        <!-- iLog Readiness -->
                                        <div class="row mt-2">
                                            <div class="col-md-3 pt-1 text-end no-right-padding">
                                                {{$t('partsmaster.columns.iLogReadinessStatus')}}
                                            </div>
                                            <div class="col-md-9 multiselect-as-select">
                                                <multiselect v-model="model.iLogReadinessStatus"
                                                             :options="iLogReadinessStatuses"
                                                             :close-on-select="true"
                                                             :searchable="false"
                                                             placeholder="select"
                                                             select-label=""
                                                             deselect-label=""
                                                             selected-label=""
                                                             :disabled="readonly"
                                                             :can-clear="false"
                                                             :can-deselect="false"
                                                             :classes="{container: 'form-control-sm multiselect'}">
                                                </multiselect>
                                                <form-error :errors="errors" field="iLogReadinessStatus" />
                                            </div>
                                        </div>
                                        <!-- Unloading point -->
                                        <div class="row mt-2">
                                            <div class="col-md-3 pt-1 text-end no-right-padding">
                                                {{$t('partsmaster.columns.unloadingPoint')}}
                                            </div>
                                            <div class="col-md-9 multiselect-as-select">
                                                <!--<select class="form-select form-select-sm multiselect-like" v-model="model.unloadingPointId" :disabled="readonly || !canEditUnloadingPointCheck">
                                                    <option v-for="uPoint in unloadingPoints" :key="uPoint.id" :value="uPoint.id">{{ uPoint.name }}</option>
                                                </select>-->

                                                <multiselect v-model="model.unloadingPointId"
                                                             :options="unloadingPointsSelect"
                                                             :close-on-select="true"
                                                             :searchable="false"
                                                             placeholder="select"
                                                             select-label=""
                                                             deselect-label=""
                                                             selected-label=""
                                                             :disabled="readonly"
                                                             :can-clear="false"
                                                             :can-deselect="false"
                                                             :classes="{container: 'form-control-sm multiselect'}">
                                                </multiselect>

                                                <form-error :errors="errors" field="unloadingPointId" />
                                            </div>
                                        </div>
                                     </div>
                                </div>
                            <!--</div>-->

                            <!-- Supply by -->
                            <!--<div class="ps-3 pt-3 pb-3">-->
                                <div class="mt-3"><strong>{{$t('partsMaster.details.supplyBy')}}</strong></div>
                                <div class="card">
                                    <div class="card-body">

                                        <!-- Supplier ID -->
                                        <div class="row">
                                            <div class="col-md-3 pt-1 text-end no-right-padding">
                                                {{$t('partsMaster.details.supplierId')}}
                                            </div>
                                            <div class="col-md-9 multiselect-as-select">
                                                <select-supplier v-model="model.supplierID" :customerCode="customerCode" :disabled="readonly"></select-supplier>
                                                <form-error :errors="errors" field="SupplierID" />
                                            </div>
                                        </div>
                                        <!-- Supplier Name -->
                                        <div class="row mt-2">
                                            <div class="col-md-3 pt-1 text-end no-right-padding">
                                                {{$t('partsMaster.details.supplierName')}}
                                            </div>
                                            <div class="col-md-9">
                                                <input type="text" :value="supplierName" class="form-control form-control-sm"  disabled/>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            <!--</div>-->

                        </div>
                        <div class="col-md-7">
                            <!-- Package Type -->
                            <!--<div class="pt-3 pe-3 pb-3">-->
                                <div><strong>{{$t('partsMaster.details.packingInformation')}}</strong></div>
                                <div class="card">
                                    <div class="card-body">
                                        <!-- Package Type -->
                                        <div class="row">
                                            <div class="col-md-2 pt-1 text-end no-right-padding">
                                                {{$t('partsMaster.details.packageType')}}
                                            </div>
                                            <div class="col-md-3 multiselect-as-select">
                                                <select-packagetype v-model="model.packageType" :disabled="readonly"></select-packagetype>
                                                <form-error :errors="errors" field="PackageType" />
                                            </div>

                                            <div class="col-md-3 pt-1 text-end no-right-padding">
                                                {{$t('partsMaster.details.palletType')}}
                                            </div>
                                            <div class="col-md-4 multiselect-as-select">
                                                <multiselect v-model="model.palletTypeId"
                                                             :options="palletTypes"
                                                             :close-on-select="true"
                                                             :searchable="false"
                                                             placeholder="select"
                                                             select-label=""
                                                             deselect-label=""
                                                             selected-label=""
                                                             :disabled="readonly"
                                                             :can-clear="false"
                                                             :can-deselect="false"
                                                             :classes="{container: 'form-control-sm multiselect'}">
                                                </multiselect>
                                            </div>
                                        </div>
                                        <div class="row mt-2">
                                            <div class="offset-md-5 col-md-3 pt-1 text-end no-right-padding">
                                                {{$t('partsMaster.details.ellisPalletType')}}
                                            </div>
                                            <div class="col-md-4 multiselect-as-select">
                                                <multiselect v-model="model.ellisPalletTypeId"
                                                             :options="ellisPalletTypes"
                                                             :close-on-select="true"
                                                             :searchable="false"
                                                             placeholder="select"
                                                             select-label=""
                                                             deselect-label=""
                                                             selected-label=""
                                                             :disabled="readonly"
                                                             :can-clear="false"
                                                             :can-deselect="false"
                                                             :classes="{container: 'form-control-sm multiselect'}">
                                                </multiselect>
                                            </div>
                                        </div>
                                        <!-- Pick by - radios -->
                                        <div class="row mt-4">
                                            <!--<div class="col-md-3">
            &nbsp;
        </div>-->
                                            <div class="col-md-4">
                                                <div class="form-check">
                                                    <input class="form-check-input" type="radio" v-model="isPickByQuantity" v-bind:value="true" name="pickby" id="pickby1" :disabled="readonly">
                                                    <label class="form-check-label" for="pickby1">
                                                        {{$t('partsMaster.details.pickByQuantity')}}
                                                    </label>
                                                </div>
                                            </div>
                                            <div class="col-md-4">
                                                <div class="form-check">
                                                    <input class="form-check-input" type="radio" v-model="isPickByQuantity" v-bind:value="false" name="pickby" id="pickby2" :disabled="readonly">
                                                    <label class="form-check-label" for="pickby2">
                                                        {{$t('partsMaster.details.pickByPackage')}}
                                                    </label>
                                                </div>
                                            </div>
                                        </div>
                                        <!-- Standard Pckg / Defected -->
                                        <div class="row">
                                            <!--<div class="col-md-3">
            &nbsp;
        </div>-->
                                            <div class="col-md-12">
                                                <div class="card">
                                                    <div class="card-body">

                                                        <!-- Standard / Defected -->
                                                        <div class="row">
                                                            <div class="col-md-3 pt-1">
                                                                <div class="form-check">
                                                                    <input class="form-check-input" v-model="model.isStandardPackaging" type="checkbox" id="flexCheckDisabled1" :disabled="readonly || isPickByQuantity">
                                                                    <form-error :errors="errors" field="IsStandardPackaging" />
                                                                    <label class="form-check-label" for="flexCheckDisabled1">
                                                                        {{$t('partsMaster.details.standardPackaging')}}
                                                                    </label>
                                                                </div>
                                                            </div>
                                                            <div class="col-md-2 pt-1">
                                                                <div class="form-check">
                                                                    <input class="form-check-input" v-model="model.isDefected" type="checkbox" id="flexCheckDisabled2" :disabled="readonly">
                                                                    <form-error :errors="errors" field="IsDefected" />
                                                                    <label class="form-check-label" for="flexCheckDisabled2">
                                                                        {{$t('partsMaster.details.defected')}}
                                                                    </label>
                                                                </div>
                                                            </div>
                                                            <div class="col-md-1 pt-1 text-end no-right-padding">
                                                                {{$t('partsMaster.details.spq')}}
                                                            </div>
                                                            <div class="col-md-2">
                                                                <input type="text" v-model="model.spq" class="form-control form-control-sm" :disabled="readonly" />
                                                                <form-error :errors="errors" field="SPQ" />
                                                            </div>

                                                            <div class="col-md-2 pt-1 text-end no-right-padding">
                                                                {{$t('partsMaster.details.orderLot')}}
                                                            </div>
                                                            <div class="col-md-2">
                                                                <input type="text" v-model="model.orderLot" class="form-control form-control-sm" :disabled="readonly" />
                                                                <form-error :errors="errors" field="OrderLot" />
                                                            </div>
                                                        </div>

                                                        <!-- SPQ -->
                                                        <div class="row mt-2">

                                                        </div>

                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mt-3">
                                            <div class="col-md-12" v-if="false">
                                                <div class="form-check">
                                                    <input class="form-check-input" v-model="model.doNotSyncEDI" type="checkbox" id="flexCheckEDI" :disabled="readonly">
                                                    <form-error :errors="errors" field="DoNotSyncEDI" />
                                                    <label class="form-check-label" for="flexCheckEDI">
                                                        {{$t('partsMaster.details.disableEdiSynchronization')}}
                                                    </label>
                                                </div>
                                            </div>
                                        </div>


                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="card">
                                                    <div class="card-body part-master-dimensions">

                                                        <div class="row">
                                                            <div class="col-md-12 no-right-padding">
                                                                <div class="float-start me-1 pt-3">{{$t('partsMaster.details.length')}}</div>
                                                                <div class="float-start w-12 me-1 text-center">
                                                                    <div class="part-master-dimensions-intext"><small>int.</small></div>
                                                                    <div class="float-start input-group input-group-sm">
                                                                        <input type="text" v-model="model.lengthInternal" class="form-control form-control-sm" aria-describedby="lenInt">
                                                                        <span class="input-group-text" id="lenInt"><small>mm</small></span>
                                                                    </div>
                                                                </div>
                                                                <div class="float-start w-11 me-1 text-center">
                                                                    <div class="part-master-dimensions-intext"><small>ext.</small></div>
                                                                    <div class="float-start input-group input-group-sm">
                                                                        <input type="text" v-model="model.length" class="form-control form-control-sm" aria-describedby="len" :disabled="readonly || !model.doNotSyncEDI">
                                                                        <span class="input-group-text" id="len"><small>cm</small></span>
                                                                    </div>
                                                                </div>


                                                                <div class="float-start ms-4 me-1 pt-3">{{$t('partsMaster.details.width')}}</div>
                                                                <div class="float-start w-12 me-1 text-center">
                                                                    <div class="part-master-dimensions-intext"><small>int.</small></div>
                                                                    <div class="float-start input-group input-group-sm">
                                                                        <input type="text" v-model="model.widthInternal" class="form-control form-control-sm" aria-describedby="widInt">
                                                                        <span class="input-group-text" id="widInt"><small>mm</small></span>
                                                                    </div>
                                                                </div>
                                                                <div class="float-start w-11 me-1 text-center">
                                                                    <div class="part-master-dimensions-intext"><small>ext.</small></div>
                                                                    <div class="float-start input-group input-group-sm">
                                                                        <input type="text" v-model="model.width" class="form-control form-control-sm" aria-describedby="wid" :disabled="readonly || !model.doNotSyncEDI">
                                                                        <span class="input-group-text" id="wid"><small>cm</small></span>
                                                                    </div>
                                                                </div>

                                                                <div class="float-start ms-4 me-1 pt-3">{{$t('partsMaster.details.height')}}</div>
                                                                <div class="float-start w-12 me-1 text-center">
                                                                    <div class="part-master-dimensions-intext"><small>int.</small></div>
                                                                    <div class="float-start input-group input-group-sm">
                                                                        <input type="text" v-model="model.heightInternal" class="form-control form-control-sm" aria-describedby="heiInt">
                                                                        <span class="input-group-text" id="widInt"><small>mm</small></span>
                                                                    </div>
                                                                </div>
                                                                <div class="float-start w-11 me-1 text-center">
                                                                    <div class="part-master-dimensions-intext"><small>ext.</small></div>
                                                                    <div class="float-start input-group input-group-sm">
                                                                        <input type="text" v-model="model.height" class="form-control form-control-sm" aria-describedby="hei" :disabled="readonly || !model.doNotSyncEDI">
                                                                        <span class="input-group-text" id="wid"><small>cm</small></span>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-md-12">
                                                                <form-error :errors="errors" field="Length" />
                                                                <form-error :errors="errors" field="LengthInternal" />
                                                                <form-error :errors="errors" field="Width" />
                                                                <form-error :errors="errors" field="WidthInternal" />
                                                                <form-error :errors="errors" field="Height" />
                                                                <form-error :errors="errors" field="HeightInternal" />
                                                            </div>
                                                        </div>

                                                        <div class="row mt-4">
                                                            <div class="col-md-6">
                                                                <div class="float-start me-1 pt-3">{{$t('partsMaster.details.netWeight')}}</div>
                                                                <div class="float-start w-25 me-1 text-center">
                                                                    <div class="part-master-dimensions-intext"><small>int.</small></div>
                                                                    <div class="float-start input-group input-group-sm">
                                                                        <input type="text" v-model="model.netWeightInternal" class="form-control form-control-sm" aria-describedby="netWeiInt">
                                                                        <span class="input-group-text" id="netWeiInt"><small>kg</small></span>
                                                                    </div>
                                                                </div>
                                                                <div class="float-start w-25 me-1 text-center">
                                                                    <div class="part-master-dimensions-intext"><small>ext.</small></div>
                                                                    <div class="float-start input-group input-group-sm">
                                                                        <input type="text" v-model="model.netWeight" class="form-control form-control-sm" aria-describedby="netWei" :disabled="readonly || !model.doNotSyncEDI">
                                                                        <span class="input-group-text" id="netWei"><small>kg</small></span>
                                                                    </div>
                                                                </div>

                                                            </div>
                                                            <div class="col-md-6 text-end">
                                                                <div class="float-end w-25 me-1 text-center">
                                                                    <div class="part-master-dimensions-intext"><small>ext.</small></div>
                                                                    <div class="float-end input-group input-group-sm">
                                                                        <input type="text" v-model="model.grossWeight" class="form-control form-control-sm" aria-describedby="groWei" :disabled="readonly || !model.doNotSyncEDI">
                                                                        <span class="input-group-text" id="groWei"><small>kg</small></span>
                                                                    </div>
                                                                </div>
                                                                <div class="float-end w-25 me-1 text-center">
                                                                    <div class="part-master-dimensions-intext"><small>int.</small></div>
                                                                    <div class="float-start input-group input-group-sm">
                                                                        <input type="text" v-model="model.grossWeightInternal" class="form-control form-control-sm" aria-describedby="groWeiInt">
                                                                        <span class="input-group-text" id="groWeiInt"><small>kg</small></span>
                                                                    </div>
                                                                </div>
                                                                <div class="float-end me-1 ms-5 pt-3">{{$t('partsMaster.details.grossWeight')}}</div>
                                                            </div>
                                                        </div>

                                                        <div class="row">
                                                            <div class="col-md-12">
                                                                <form-error :errors="errors" field="NetWeight" />
                                                                <form-error :errors="errors" field="NetWeightInternal" />
                                                                <form-error :errors="errors" field="GrossWeight" />
                                                                <form-error :errors="errors" field="GrossWeightInternal" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>


                                        <!-- Pallet item -->
                                        <div class="row mt-3">
                                            <div class="col-md-12">
                                                <div class="card">
                                                    <div class="card-body">
                                                        <div class="row">
                                                            <div class="col-md-12 mb-2">
                                                                <div>
                                                                    <div>
                                                                        <input class="form-check-input me-2" type="checkbox" v-model="model.isMixed" id="mixed" @click="mixedOrCpart(true)" :disabled="readonly">
                                                                        <label class="form-check-label" for="mixed">
                                                                            {{$t('partsMaster.details.isMixed')}}
                                                                        </label>
                                                                    </div>
                                                                    <form-error :errors="errors" field="IsCPart" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-md-5">
                                                                <div class="float-start">
                                                                    <div class="pt-1 no-right-padding">
                                                                        <div>
                                                                            <div>
                                                                                <input class="form-check-input me-2" type="checkbox" v-model="model.isCPart" id="cpart" @click="mixedOrCpart(false)" :disabled="readonly">
                                                                                <label class="form-check-label" for="cpart">
                                                                                    {{$t('partsMaster.details.cpartSPQ')}}
                                                                                </label>
                                                                            </div>
                                                                            <form-error :errors="errors" field="IsCPart" />
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="float-start w-50 ms-2">
                                                                    <div>
                                                                        <input type="text" class="form-control form-control-sm" v-model="model.cPartSPQ" :disabled="readonly || !model.isCPart" />
                                                                        <form-error :errors="errors" field="CPartSPQ" />
                                                                    </div>
                                                                </div>
                                                            </div>


                                                            <div class="col-md-7 no-right-padding">
                                                                <div class="row">
                                                                    <div class="col-md-12">
                                                                        <div class="float-start pt-1 no-right-padding">
                                                                            <div class="form-check">
                                                                                <input class="form-check-input" type="checkbox" v-model="model.isPalletItem" id="palletItem" :disabled="readonly">
                                                                                <form-error :errors="errors" field="IsPalletItem" />
                                                                                <label class="form-check-label" for="palletItem">
                                                                                    {{$t('partsMaster.details.palletItem')}}
                                                                                </label>
                                                                            </div>
                                                                        </div>
                                                                        <div class="float-start pt-1 no-right-padding ms-3">
                                                                            <div class="form-check">
                                                                                <input class="form-check-input" type="checkbox" v-model="model.boxItem" id="boxItem" :disabled="readonly">
                                                                                <form-error :errors="errors" field="BoxItem" />
                                                                                <label class="form-check-label" for="boxItem">
                                                                                    {{$t('partsMaster.details.boxItem')}}
                                                                                </label>
                                                                            </div>
                                                                        </div>
                                                                        <div class="float-start pt-1 no-right-padding ms-3">
                                                                            <div class="form-check">
                                                                                <input class="form-check-input" type="checkbox" v-model="model.masterSlave" id="masterSlave" :disabled="readonly">
                                                                                <form-error :errors="errors" field="MasterSlave" />
                                                                                <label class="form-check-label" for="masterSlave">
                                                                                    {{$t('partsMaster.details.masterSlave')}}
                                                                                </label>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                        </div>

                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row mt-4">
                                            <div class="col-md-4">
                                                <div class="row">
                                                    <div class="col-md-7 pt-1 text-end no-right-padding">
                                                        {{$t('partsMaster.details.floorStackability')}}
                                                    </div>
                                                    <div class="col-md-5">
                                                        <input type="number" class="form-control form-control-sm" v-model="model.floorStackability" :disabled="readonly" />
                                                        <form-error :errors="errors" field="FloorStackability" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-4">
                                                <div class="row">
                                                    <div class="col-md-7 pt-1 text-end no-right-padding">
                                                        {{$t('partsMaster.details.truckStackability')}}
                                                    </div>
                                                    <div class="col-md-5">
                                                        <input type="number" class="form-control form-control-sm" v-model="model.truckStackability" :disabled="readonly" />
                                                        <form-error :errors="errors" field="TruckStackability" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-4">
                                                <div class="row">
                                                    <div class="col-md-7 pt-1 text-end no-right-padding">
                                                        {{$t('partsMaster.details.boxesInPallet')}}
                                                    </div>
                                                    <div class="col-md-5">
                                                        <input type="number" class="form-control form-control-sm" v-model="model.boxesInPallet" :disabled="readonly" />
                                                        <form-error :errors="errors" field="BoxesInPallet" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            <!--</div>-->
                        </div>
                    </div>

                    

                </div>
                <div class="tab-pane fade" id="supply" role="tabpanel" aria-labelledby="supply-tab">...</div>
            </div>

        </template>
        <template name="footer" v-slot:footer>
            <button class="btn btn-primary btn-sm me-2" type="button" @click.stop="process()" :disabled="processing || readonly">
                <i class="las la-check-circle"></i> {{$t('partsmaster.operation.ok')}}
            </button>
            <button class="btn btn-secondary btn-sm me-2" type="button" @click.stop="$emit('close')" :disabled="processing">
                <i class="las la-times"></i> {{$t('partsmaster.operation.close')}}
            </button>
        </template>
    </modal>
</template>
<script>
    import Multiselect from '@vueform/multiselect'
    import SelectUom from '@/widgets/SelectUOMWithDecimal.vue'
    import SelectCountry from '@/widgets/SelectCountry.vue'
    import SelectSupplier from '@/widgets/SelectSupplier.vue'
    import FormError from '@/widgets/FormError.vue'
    import SelectPackagetype from '@/widgets/SelectPackageType.vue'
    import { defineComponent } from 'vue';
    import { useStore } from '@/store';
    export default defineComponent({
        components: { SelectUom, SelectCountry, SelectPackagetype, Multiselect, SelectSupplier, FormError },
        props: ['customerCode', 'productCode1', 'supplierID', 'type', 'productCodeMap'],
        created() {
            if (this.type != 'new') {
                this.$dataProvider.partMasters.getPartMaster(this.customerCode, this.productCode1, this.supplierID)
                    .then((resp) => {
                        this.model = resp;
                        // convert to mm for UI purposes
                        this.model.lengthInternal = resp.lengthInternal && !isNaN(resp.lengthInternal) ? resp.lengthInternal * 10 : null;
                        this.model.widthInternal = resp.widthInternal && !isNaN(resp.widthInternal) ? resp.widthInternal * 10 : null;
                        this.model.heightInternal = resp.heightInternal && !isNaN(resp.heightInternal) ? resp.heightInternal * 10 : null;
                    });
                this.getUnloadingPoints(this.customerCode, this.supplierID);
            }
            this.$dataProvider.supplierMasters.getList(this.customerCode).then(resp => this.suppliers = resp)
            this.getILogReadinessStatuses();
            this.getPalletTypes();
            this.getEllisPalletTypes();
        },
        data() {
            return {
                statuses: { 1: 'Active', 0: 'InActive' },
                suppliers: [],
                processing: false,
                isPickByQuantity: false,
                iLogReadinessStatuses: {},
                palletTypes: {},
                ellisPalletTypes: {},
                unloadingPoints: {},
                unloadingPointsSelect: [],
                canEditUnloadingPointCheck: useStore().state.canEditUnloadingPoint,
                model: {
                    productCode1: null,
                    productCode2: null,
                    productCode3: null,
                    productCode4: null,
                    description: null,
                    uom: null,
                    originCountry: null,
                    status: 1,
                    enableSerialNo: true,
                    packageType: null,
                    isStandardPackaging: false,
                    isDefected: false,
                    spq: 0,
                    orderLot: 1,
                    length: 1,
                    lengthInternal: 1,
                    width: 1,
                    widthInternal: 1,
                    height: 1,
                    heightInternal: 1,
                    netWeight: 1,
                    grossWeight: 1,
                    isPalletItem: false,
                    isCPart: false,
                    cPartSPQ: 0,
                    supplierID: null,
                    customerCode: null,
                    boxItem: false,
                    masterSlave: false,
                    floorStackability: 0,
                    truckStackability: 0,
                    boxesInPallet: 0,
                    doNotSyncEDI: false,
                    iLogReadinessStatus: "Not registered",
                    isMixed: false,
                    palletTypeId: null,
                    ellisPalletTypeId: null,
                    unloadingPointId: null
                },
                errors: {}
            }
        },
        methods: {
            process() {
                this.processing = true
                this.model.customerCode = this.customerCode
                const modelToUpdate = Object.assign({}, this.model);
                modelToUpdate.lengthInternal = this.model.lengthInternal / 10;
                modelToUpdate.widthInternal = this.model.widthInternal / 10;
                modelToUpdate.heightInternal = this.model.heightInternal / 10;
                if (this.type == 'modify') {
                    this.$dataProvider.partMasters.updatePartMaster(modelToUpdate)
                        .then(() => this.$emit('close'))
                        .catch(e => {
                            if (e.response && e.response.data.errors) {
                                this.errors = e.response.data.errors
                            }
                        })
                        .finally(() => this.processing = false)
                }
                else if (this.type == 'new') {
                    this.$dataProvider.partMasters.createPartMaster(modelToUpdate)
                        .then(() => this.$emit('close'))
                        .catch(e => {
                            if (e.response && e.response.data.errors) {
                                this.errors = e.response.data.errors
                            }
                        })
                        .finally(() => this.processing = false)
                }
            },
            getILogReadinessStatuses() {
                this.$dataProvider.partMasters.iLogReadinessStatuses().then((statuses) => {
                    this.iLogReadinessStatuses = statuses;
                })
            },
            getPalletTypes() {
                this.$dataProvider.partMasters.getPalletTypes().then((types) => {
                    this.palletTypes = types.data.map(x => ({ value: x.id, label: x.name }));
                    if (this.type == 'new') {
                        let nonstd = types.data.find(x => x.name == 'NOT STD')
                        this.model.palletTypeId = nonstd ? nonstd.id : (types.data[0] ? types.data[0].id : null);
                    }
                })
            },
            getEllisPalletTypes() {
                this.$dataProvider.partMasters.getEllisPalletTypes().then((ellisTypes) => {
                    this.ellisPalletTypes = ellisTypes.data.map(x => ({ value: x.id, label: x.name }));
                    if (this.type == 'new') { this.model.ellisPalletTypeId = 0; }
                })
            },
            mixedOrCpart(isMixed) {
                this.model.isMixed = isMixed ? true : false;
                this.model.isCPart = !this.model.isMixed;
            },
            getUnloadingPoints(customerCode, supplierId) {
                this.unloadingPoints = {};
                this.$dataProvider.partMasters.getUnloadingPoints(customerCode, supplierId).then((uPoints) => {
                    this.unloadingPoints = uPoints.data.options;
                    this.unloadingPointsSelect = this.unloadingPoints.map(x => ({ value: x.id, label: x.name }));
                    if (this.type == 'new') {
                        this.model.unloadingPointId = uPoints.data.defaultId;
                    }
                })
            }
        },
        computed: {
            supplierName() {
                let t = this.suppliers.find(x => x.supplierID == this.model.supplierID)
                return t ? t.companyName : ""
            },
            readonly() {
                return this.type == 'view'
            },
            title() {
                return this.$t(`partsmaster.detail.title_${this.type}`)
            }
        },
        watch: {
            isPickByQuantity(v) {
                this.model.isStandardPackaging = !v
            },
            'model.supplierID': {
                handler(after, before) {
                    if (before != after) {
                        this.getUnloadingPoints(this.customerCode, after);
                    }
                }, deep: true
            }
        }
    })
</script>
<style lang="scss">
    .modal-partsmaster-details {
        .modal-container {
            width: 90vw;
        }
    }
</style>