namespace apps.systems {

    export interface perfermanceCount {
        svr: Service;
        Counter: number[];
    }

    export interface counterData {
        x: number[];
        y: number[];
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

        private cpu_chart: plot.histogramPlot<counterData>;
        private mem_chart: plot.histogramPlot<counterData>;
        private refresh: boolean;

        private onDraw() {
            const vm = this;

            if (!vm.plot) {
                return;
            } else if (!(vm.cpu_chart && vm.mem_chart)) {
                return;
            }

            const cpu = vm.plot.cpu;
            const mem = vm.plot.memory;
            const panel = $ts("#service-info").clear();
            const x: number[] = [...cpu.Counter].map((_, index) => index + 1);

            panel.display($ts("<h3>").display(cpu.svr.Name));
            panel.appendElement($ts("<p>").display(cpu.svr.Description));
            panel.appendElement($ts("<p>").display(cpu.svr.StartTime));

            if (this.refresh) {
                this.cpu_chart.plot(<counterData>{ x: x, y: cpu.Counter });
                this.mem_chart.plot(<counterData>{ x: x, y: mem.Counter });
            } else {
                this.cpu_chart.chartObj.setOption({ series: [{ data: servicesManager.history(x, cpu.Counter) }] });
                this.mem_chart.chartObj.setOption({ series: [{ data: servicesManager.history(x, mem.Counter) }] });
            }

            this.refresh = false;
        }

        private static history(x: number[], y: number[]): number[][] {
            const bars: number[][] = [];

            for (let i = 0; i < x.length; i++) {
                bars.push([x[i], y[i]]);
            }

            return bars;
        }

        private static counterChart(data: counterData): plot.histogram_options {
            return <plot.histogram_options><any>{
                title: {
                    text: 'Performance Counter'
                },
                xAxis: {
                    type: 'value',
                    splitLine: {
                        show: true
                    }
                },
                yAxis: {
                    type: 'value',
                    boundaryGap: [0, '100%'],
                    splitLine: {
                        show: true
                    }
                },
                series: [
                    {
                        name: 'Performance Counter',
                        type: 'line',
                        showSymbol: false,
                        data: servicesManager.history(data.x, data.y)
                    }
                ]
            };
        }

        private updatePlotHost() {
            this.cpu_chart = new plot.histogramPlot<counterData>(servicesManager.counterChart, "cpu-history");
            this.mem_chart = new plot.histogramPlot<counterData>(servicesManager.counterChart, "mem-history");
            this.refresh = true;
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

                if (vm.plot) {
                    if (vm.plot.cpu.svr.PID != cpu.svr.PID) {
                        // create new plot
                        vm.updatePlotHost();
                    }
                } else {
                    // create new plot
                    vm.updatePlotHost();
                }

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