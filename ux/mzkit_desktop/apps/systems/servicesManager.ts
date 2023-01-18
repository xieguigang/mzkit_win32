namespace apps.systems {

    export class servicesManager extends Bootstrap {

        public get appName(): string {
            return "mzkit/services";
        };

        protected init(): void {
            setInterval(() => this.startUpdateTask(), 1000);
        }

        private startUpdateTask() {
            const vm = this;

            app.desktop.mzkit
                .GetServicesList()
                .then(async function (json: string) {
                    const fetch: string = await json;
                    const list: Service[] = JSON.parse(fetch);

                    vm.loadServicesList(list);
                });
        }

        private loadServicesList(list: Service[]) {
            $ts("#services-list").clear();
            $ts.appendTable(list, "#services-list", null, { class: [] }, servicesManager.styleEachRow);
        }

        private static styleEachRow(svr: Service, row: HTMLTableRowElement) {
            if (!svr.isAlive) {
                row.classList.add("disabled");
            }
        }
    }

    export interface Service {
        name: string;
        description: string;
        port: number;
        PID: number;
        cpu: number;
        mem: number;
        isAlive: boolean;
    }

}