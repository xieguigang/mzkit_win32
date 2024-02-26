namespace apps.systems {

    const pages: {} = {
        "mzkit_page": "MZKit Settings",
        "msraw_page": "Raw File Viewer",
        "chromagram_page": "Chromagram Plot Styles",
        "formula_page": "Formula Search",
        "element_profile_page": "Formula Search Profile"
    };

    export interface BootstrapTable {
        bootstrapTable(arg1: any, arg2?: any);
    }

    export class settings extends Bootstrap {

        get appName(): string {
            return "mzkit/settings";
        }

        protected init(): void {
            this.mzkit_page_btn_onclick();
            this.load_profileTable();
        }

        private load_profileTable() {
            const bootstrap: BootstrapTable = <any>$("#tableDiv");

            let data = [{
                id: 1,
                month: 1,
                department: "技术部",
                fee: 10090,
                comment: "comment"
            }, {
                id: 1,
                month: 2,
                department: "管理中心",
                fee: 19000,
                comment: "备注"
            }]
            let columns = [{
                title: "编号",
                field: "id",
                sortable: true,
                width: 200,
                editable: false,
            }, {
                title: "月份",
                field: "month",
                sortable: true,
                width: 200,
                formatter: function (v) {
                    return v + "月"
                },
                editable: {
                    type: "select",
                    options: {
                        items: [{
                            value: 1,
                            label: "1月",
                        }, {
                            value: 2,
                            label: "2月",
                        }, {
                            value: 3,
                            label: "3月",
                        }, {
                            value: 4,
                            label: "4月",
                        }, {
                            value: 5,
                            label: "5月",
                        }]
                    }
                }
            }, {
                title: "部门",
                field: "department",
                sortable: true,
                width: 200,
                editable: {
                    type: "select",
                    options: {
                        items: [
                            "技术部", "生产部", "管理中心"
                        ]
                    }
                }
            }, {
                title: "管理费用",
                field: "fee",
                sortable: true,
                width: 200,
                editable: {
                    type: "number"
                }
            }, {
                title: "备注",
                field: "comment",
                sortable: true,
                width: 200,
                editable: true,
                // editable:{
                //   type:"text"
                // }
            },];

            let tableOptions = {
                columns: columns,
                editable: true, //editable需要设置为 true
            }
            bootstrap.bootstrapTable(tableOptions);
            bootstrap.bootstrapTable("load", data);
        }

        private static closeAll(): typeof settings {
            for (let page of Object.keys(pages)) {
                $ts(`#${page}`).hide();
            }

            return this;
        }

        private static show(page_id: string) {
            $ts(`#${page_id}`).show();
            $ts("#title").display(pages[page_id]);
        }

        public mzkit_page_btn_onclick() {
            settings.closeAll().show("mzkit_page");
        }

        public msraw_btn_onclick() {
            settings.closeAll().show("msraw_page");
        }

        public chromagram_btn_onclick() {
            settings.closeAll().show("chromagram_page");
        }

        public formula_btn_onclick() {
            settings.closeAll().show("formula_page");
        }

        public profile_btn_onclick() {
            settings.closeAll().show("element_profile_page");
        }
    }
}