import HelloWorld from './components/HelloWorld.vue'
import StoreTest from './components/StoreTest.vue'
import StoreTest1 from './components/StoreTest1.vue'
import Login from './components/Login.vue'
import Dashboard from './components/Dashboard.vue'
import Help from './components/Help.vue'
import NotFound from './components/NotFound.vue'
import Outbound from './components/Outbound.vue'
import Inbound from './components/Inbound.vue'
import StorageGroup from './components/StorageDetailGroup.vue'
import InboundDetail from './components/InboundDetail.vue'
import Loading from './components/Loading.vue'
import OutboundDetail from './components/OutboundDetail.vue'
import LoadingDetail from './components/LoadingDetail.vue'
import Frame from './components/Frame.vue'
import PartsMaster from './components/PartsMaster.vue'
import UserManagement from './components/UserManagement.vue'
import StockTransfer from './components/StockTransfer.vue'
import StockTransferDetail from './components/StockTransferDetail.vue'
import Inventory from './components/inventory/Inventory.vue'
import InventoryAdjustment from './components/inventory/screens/Adjustment.vue'
import StorageDetail from './components/inventory/screens/StorageDetail.vue'
import InventoryAdjustmentItem from './components/inventory/screens/AdjustmentItem.vue'
import Quarantine from './components/inventory/screens/Quarantine.vue'
import Relocation from './components/inventory/screens/Relocation.vue'
import Decant from './components/inventory/screens/Decant.vue'
import DecantDetails from './components/inventory/screens/DecantDetails.vue'
import Reports from './components/inventory/screens/Reports.vue'
import Invoicing from './components/invoicing/Invoicing.vue'
import iLogIntegrationManagement from './components/ilogintegrationmanagement/iLogIntegrationManagement.vue'
import InboundReversal from './components/inboundReversal/InboundReversal.vue'
import InboundReversalDetail from './components/inboundReversal/InboundReversalDetail.vue'
import StockTransferReversal from './components/stockTransferReversal/StockTransferReversal.vue'
import StockTransferReversalDetail from './components/stockTransferReversal/StockTransferReversalDetail.vue'
import CompanyProfile from './components/companyProfile/CompanyProfile.vue'
import Customer from './components/customer/Customer.vue'
import CustomerUom from './components/customer/CustomerUom.vue'
import CustomerClient from './components/customer/CustomerClient.vue'
import StockTake from './components/stockTake/StockTake.vue'
import StockTakeDetail from './components/stockTake/StockTakeDetail.vue'
import Settings from './components/settings/Settings.vue'
import SatoPrinter from './components/settings/SatoPrinter.vue'
import Area from './components/settings/Area.vue'
import AreaType from './components/settings/AreaType.vue'
import ControlCode from './components/settings/ControlCode.vue'
import LocationSettings from './components/settings/Location.vue'
import ProductCode from './components/settings/ProductCode.vue'
import PackageType from './components/settings/PackageType.vue'
import Uom from './components/settings/Uom.vue'
import Warehouse from './components/settings/Warehouse.vue'

export default [
    
    {
        path: '/',
        component: Frame,
        meta: {
                auth: true
              },
        children: [{
                path: '/hello', component: HelloWorld,
                name: 'test',
                showMenu: true,
                meta: {
                    auth: true,
                    title: 'Test page',
                    showMenu: true
                }
            },
            {
                path: '/inventory', component: Inventory,
                name: 'inventory',
                meta: {
                    auth: true,
                    title: 'Inventory',
                    showMenu: true
                }
            },
            {
                path: '/invoicing', component: Invoicing,
                name: 'invoicing',
                meta: {
                    auth: true,
                    title: 'Invoicing',
                    showMenu: true
                }
            },
            {
                path: '/adjustment', component: InventoryAdjustment,
                name: 'adjustment',
                meta: {
                    auth: true,
                    title: 'Inventory Adjustment',
                    showMenu: true
                }
            },
            {
                path: '/adjustment_item/:jobNo', component: InventoryAdjustmentItem,
                name: 'adjustment-item',
                meta: {
                    auth: true,
                    title: (c) => {
                        return c.params.jobNo != 'new' ? c.params.jobNo : 'Add new item'
                    },
                    key(route) {
                        return 'decant-' + route.params.jobNo
                    },
                    showMenu: true
                }
            },
            {
                path: '/storage_detail', component: StorageDetail,
                name: 'storage_detail',
                meta: {
                    auth: true,
                    title: 'Storage Detail',
                    showMenu: true
                }
            },
            {
                path: '/quarantine', component: Quarantine,
                name: 'quarantine',
                meta: {
                    auth: true,
                    title: 'Quarantine',
                    showMenu: true
                }
            },
            {
                path: '/relocation', component: Relocation,
                name: 'relocation',
                meta: {
                    auth: true,
                    title: 'Relocation',
                    showMenu: true
                }
            },
            {
                path: '/decant', component: Decant,
                name: 'decant',
                meta: {
                    auth: true,
                    title: 'Decant',
                    showMenu: true
                }
            },
            {
                path: '/decant_details/:jobNo', component: DecantDetails,
                name: 'decant_details',
                meta: {
                    auth: true,
                    title: (c) => {
                        return c.params.jobNo
                    },
                    key(route) {
                        return 'decant-' + route.params.jobNo
                    },
                    showMenu: true
                }
            },
            {
                path: '/reports', component: Reports,
                name: 'reports',
                meta: {
                    auth: true,
                    title: 'Reports',
                    showMenu: true
                }
            },
            {
                path: '/store', component: StoreTest,
                name: 'teststore',
                meta: {
                    auth: true,
                    title: 'Test store page',
                    showMenu: true
                }
            },
            {
                path: '/store1', component: StoreTest1,
                name: 'teststore1',
                meta: {
                    auth: true,
                    title: 'Test store page',
                    showMenu: true
                }
            },
            {
                path: '/',
                component: Dashboard,
                name: 'home',
                meta: {
                    auth: true,
                    key: 'home',
                    title: 'Homepage',
                    closable: false,
                    showMenu: true
                }
            },
            {
                path: '/outbound',
                component: Outbound,
                name: 'outbound',
                meta: {
                    auth: true,
                    title: 'Outbounds',
                    accessRight: 'OUTBOUND',
                    showMenu: true
                }
            },
            {
                path: '/outbound/:jobNo',
                component: OutboundDetail,
                name: 'outbound_detail',
                props: true,
                meta: {
                    auth: true,
                    title: (c) => {
                        return c.params.jobNo
                    },
                    key(route) {
                        return 'outbound-' + route.params.jobNo
                    },
                    showMenu: true
                }
            },
            {
                path: '/inbound',
                component: Inbound,
                name: 'inbound',
                meta: {
                    auth: true,
                    title: 'Inbounds',
                    accessRight: 'INBOUND',
                    showMenu: true
                }
            },
            {
                path: '/storage_group',
                component: StorageGroup,
                name: 'storage-group',
                meta: {
                    auth: true,
                    title: 'Storage Groups',
                    accessRight: 'INBOUND',
                    showMenu: true
                }
            },
            {
                path: '/parts_master',
                component: PartsMaster,
                name: 'partsmaster',
                meta: {
                    auth: true,
                    title: 'Parts Master',
                    showMenu: true
                }
            },
            {
                path: '/inbound/:jobNo',
                component: InboundDetail,
                name: 'inbound_detail',
                props: true,
                meta: {
                    auth: true,
                    title: (c) => {
                        return c.params.jobNo
                    },
                    key(route) {
                        return 'outbound-' + route.params.jobNo
                    },
                    showMenu: true
                }
            },
            {
                path: '/loading',
                component: Loading,
                name: 'loading',
                meta: {
                    auth: true,
                    title: 'Loadings',
                    accessRight: 'LOADING',
                    showMenu: true
                }
            },
            {
                path: '/loading/:jobNo',
                component: LoadingDetail,
                name: 'loading_detail',
                props: true,
                meta: {
                    auth: true,
                    title: (c) => {
                        return c.params.jobNo
                    },
                    key(route) {
                        return 'loading-' + route.params.jobNo
                    },
                    showMenu: true
                }
            },
            {
                path: '/users',
                component: UserManagement,
                name: 'usermanagement',
                meta: {
                    auth: true,
                    title: 'User Management',
                    accessRight: ['USER', 'GROUP'],
                    showMenu: true
                }
            },
            {
                path: '/stockTransfer',
                component: StockTransfer,
                name: 'stocktransfer',
                meta: {
                    auth: true,
                    title: 'Stock Transfer',
                    accessRight: 'STOCKTRANSFER',
                    showMenu: true
                }
            },
            {
                path: '/stockTransfer/:jobNo',
                component: StockTransferDetail,
                name: 'stocktransfer_detail',
                props: true,
                meta: {
                    auth: true,
                    title: (c) => {
                        return c.params.jobNo
                    },
                    key(route) {
                        return 'stocktransfer-' + route.params.jobNo
                    },
                    showMenu: true
                }
            },
            {
                path: '/help',
                component: Help,
                name: 'help',
                meta: {
                    auth: true,
                    title: 'Help',
                    showMenu: true
                }
            },
            {
                path: '/inboundReversal', component: InboundReversal,
                name: 'inboundReversal',
                meta: {
                    auth: true,
                    title: 'Inbound Reversal',
                    showMenu: true
                }
            },
            {
                path: '/inboundReversalDetail/:jobNo', component: InboundReversalDetail,
                name: 'inboundReversal-detail',
                meta: {
                    auth: true,
                    title: (c) => {
                        return c.params.jobNo != 'new' ? c.params.jobNo : 'Add new item'
                    },
                    showMenu: true
                }
            },
            {
                path: '/stockTransferReversal', component: StockTransferReversal,
                name: 'stockTransferReversal',
                meta: {
                    auth: true,
                    title: 'Stock Transfer Reversal',
                    showMenu: true
                }
            },
            {
                path: '/stockTransferReversalDetail/:jobNo', component: StockTransferReversalDetail,
                name: 'stockTransferReversal-detail',
                meta: {
                    auth: true,
                    title: (c) => {
                        return c.params.jobNo != 'new' ? c.params.jobNo : 'Add new item'
                    },
                    showMenu: true
                }
            },
            {
                path: '/companyProfile', component: CompanyProfile,
                name: 'company-profile',
                meta: {
                    auth: true,
                    title: 'Company Profile',
                    showMenu: true,
                    accessRight: 'COMPANYPROFILE'
                }
            },
            {
                path: '/customer', component: Customer,
                name: 'customer',
                meta: {
                    auth: true,
                    title: 'Customer',
                    showMenu: true,
                    accessRight: 'CUSTOMER'
                }
            },
            {
                path: '/customerUom/:customerCode', component: CustomerUom,
                name: 'customer-uom',
                meta: {
                    auth: true,
                    title: (c) => {
                        return c.params.customerCode != 'new' ? ('UOM for ' + c.params.customerCode) : 'Add new'
                    },
                    showMenu: true,
                    accessRight: 'CUSTOMERUOM'
                }
            },
            {
                path: '/customerClient/:customerCode', component: CustomerClient,
                name: 'customer-client',
                meta: {
                    auth: true,
                    title: (c) => {
                        return c.params.customerCode != 'new' ? (c.params.customerCode + ' Clients') : 'Add new'
                    },
                    showMenu: true,
                    accessRight: 'CLIENT'
                }
            },
            {
                path: '/settings', component: Settings,
                name: 'settings',
                meta: {
                    auth: true,
                    title: 'Settings',
                    showMenu: true,
                    accessRight: 'CLIENT'
                }
            },
            {
                path: '/satoPrinter', component: SatoPrinter,
                name: 'sato-printer',
                meta: {
                    auth: true,
                    title: 'SATO Printer',
                    showMenu: true,
                    accessRight: 'CLIENT'
                }
            },
            {
                path: '/area', component: Area,
                name: 'area',
                meta: {
                    auth: true,
                    title: 'Area',
                    showMenu: true,
                    accessRight: 'CLIENT'
                }
            },
            {
                path: '/areaType', component: AreaType,
                name: 'area-type',
                meta: {
                    auth: true,
                    title: 'Area Type',
                    showMenu: true,
                    accessRight: 'CLIENT'
                }
            },
            {
                path: '/controlCode', component: ControlCode,
                name: 'control-code',
                meta: {
                    auth: true,
                    title: 'Control Code',
                    showMenu: true,
                    accessRight: 'CLIENT'
                }
            },
            {
                path: '/location', component: LocationSettings,
                name: 'location',
                meta: {
                    auth: true,
                    title: 'Location',
                    showMenu: true,
                    accessRight: 'CLIENT'
                }
            },
            {
                path: '/productCode', component: ProductCode,
                name: 'product-code',
                meta: {
                    auth: true,
                    title: 'Product Code',
                    showMenu: true,
                    accessRight: 'CLIENT'
                }
            },
            {
                path: '/packageType', component: PackageType,
                name: 'package-type',
                meta: {
                    auth: true,
                    title: 'Package Type',
                    showMenu: true,
                    accessRight: 'CLIENT'
                }
            },
            {
                path: '/uom', component: Uom,
                name: 'uom',
                meta: {
                    auth: true,
                    title: 'UOM',
                    showMenu: true,
                    accessRight: 'CLIENT'
                }
            },
            {
                path: '/warehouse', component: Warehouse,
                name: 'warehouse',
                meta: {
                    auth: true,
                    title: 'Warehouse',
                    showMenu: true,
                    accessRight: 'CLIENT'
                }
            },
            {
                path: '/stockTake', component: StockTake,
                name: 'stock-take',
                meta: {
                    auth: true,
                    title: 'Stock Take',
                    showMenu: true,
                    accessRight: ''
                }
            },
            {
                path: '/stockTake/:jobNo', component: StockTakeDetail,
                name: 'stock-take-detail',
                meta: {
                    auth: true,
                    title: (c) => {
                        return c.params.customerCode != 'new' ? (c.params.jobNo) : 'Add new'
                    },
                    showMenu: true,
                    accessRight: ''
                }
            }
        ]
    },
    {
        path: '/login',
        component: Login,
        name: 'login',
        meta: {
            auth: false,
            baseClass: 'base base-login',
            key: 'home',
            title: 'Homepage',
            showMenu: false
        }
    },
    {
        path: '/ilogintegrationmanagement', component: iLogIntegrationManagement,
        name: 'ilogintegrationmanagement',
        meta: {
            auth: true,
            title: 'iLog Integration Management',
            showMenu: false
        }
    },
    {
        path: '/:catchAll(.*)',
        component: NotFound,
        meta: {
            showMenu: true
        }
    }
]