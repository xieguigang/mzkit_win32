namespace apps.systems {

    export interface perfermanceCount {
        svr: Service;
        Counter: number[];
    }

    export class servicesManager extends Bootstrap {

        public get appName(): string {
            return "mzkit/services";
        };

        readonly cpu = new Dictionary<perfermanceCount>();
        readonly memory = new Dictionary<perfermanceCount>();

        private plot: {
            cpu: perfermanceCount,
            memory: perfermanceCount
        };

        protected init(): void {
            this.startUpdateTask();
            setInterval(() => this.startUpdateTask(), 1500);
        }

        /**
         * on update a frame display
        */
        private startUpdateTask() {
            const vm = this;

            app.desktop.mzkit
                .GetServicesList()
                .then(async function (json: string) {
                    const fetch: string = await json;
                    const list: Service[] = JSON.parse(fetch);

                    vm.loadServicesList(list);
                    vm.onDraw();
                });
        }

        private loadServicesList(list: Service[]) {
            const vm = this;

            for (let svr of list) {
                const id = `P${svr.PID}`;

                if (!vm.cpu.ContainsKey(id)) {
                    vm.cpu.Add(id, <perfermanceCount>{ svr: svr, Counter: [] });
                    vm.memory.Add(id, <perfermanceCount>{ svr: svr, Counter: [] });
                }

                if (svr.isAlive) {
                    vm.cpu.Item(id).Counter.push(svr.CPU);
                    vm.memory.Item(id).Counter.push(<number>svr.Memory);
                }

                svr.Memory = Strings.Lanudry(<number>svr.Memory);
                svr.isAlive = svr.isAlive ? "Running" : "Stopped";
            }

            $ts("#num-svr").display(list.length.toString());
            $ts("#services-list").clear();
            $ts.appendTable(list, "#services-list", null, { class: [] }, (o, r) => vm.styleEachRow(o, r));
        }

        private onDraw() {
            const vm = this;

            if (!vm.plot) {
                return;
            }

            const cpu = vm.plot.cpu;
            const mem = vm.plot.memory;
            const x: number[] = [...cpu.Counter].map((_, index) => index + 1);
            const panel = $ts("#service-info").clear();

            panel.display($ts("<h3>").display(cpu.svr.Name));
            panel.appendElement($ts("<p>").display(cpu.svr.Description));
            panel.appendElement($ts("<p>").display(cpu.svr.StartTime));
        }

        private styleEachRow(svr: Service, row: HTMLTableRowElement) {
            const vm = this;

            if (!(svr.isAlive == "Running")) {
                row.classList.add("disabled");
            }

            row.onclick = function () {
                const id: string = `P${svr.PID}`;
                const cpu = vm.cpu.Item(id);
                const mem = vm.memory.Item(id);

                vm.plot = { cpu: cpu, memory: mem };
                // draw echart
                vm.onDraw();
            }
        }
    }

    export interface Service {
        Name: string;
        Description: string;
        Port: number;
        PID: number;
        CPU: number;
        Memory: number | string;
        isAlive: boolean | string;
        StartTime: string;
    }

}