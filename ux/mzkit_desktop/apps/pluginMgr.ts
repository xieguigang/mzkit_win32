namespace apps {

    export class pluginMgr extends Bootstrap {

        public get appName(): string {
            return "pluginMgr";
        };

        protected init(): void {
            const vm = this;

            app.desktop.mzkit
                .GetPlugins()
                .then(async function (json) {
                    const json_str: string = await json;
                    const list: plugin[] = JSON.parse(json_str);
                    const mgr: HTMLElement = $ts("#plugin-list").clear();

                    console.log("get plugin list:");
                    console.table(list);
                    console.log("json string source:");
                    console.log(json_str);

                    for (let plugin of list) {
                        vm.addPlugin(mgr, plugin);
                    }
                });
        }

        private addPlugin(mgr: HTMLElement, plugin: plugin) {
            const type: string = (plugin.status == "disable" || plugin.status == "incompatible") ? "inactive" : "active";
            const row = $ts("<tr>", { class: type });
            const html: string = `
            
            <th scope="row" class="check-column">
                <input type="checkbox" name="check_plugins" >
            </th>
            <td class="plugin-title column-primary">
                <strong>${plugin.name}</strong>
                <div class="row-actions visible">
                    <span class="activate">
                        <a href="#" class="edit">Activate</a> |
                    </span>
                    <span class="delete">
                        <a href="#" class="delete">Delete</a>
                    </span>
                </div>
        
            </td>
            <td class="column-description desc">
                <div class="plugin-description">
                    <p>
                        ${plugin.desc}
                    </p>
                </div>
                <div class="${type} second plugin-version-author-uri">
                    Version ${plugin.ver} | By
                    <a href="#">${plugin.author}</a> |
                    <a href="${plugin.url}" class="thickbox open-plugin-details-modal">View details</a>
                </div>
            </td>
            <td class="column-auto-updates">
                {$usage_stat}
            </td>     
            `;

            mgr.appendChild(row.display(html));
        }

        public install_local_onclick() {
            app.desktop.mzkit.InstallLocal();
        }
    }

    export interface plugin {
        id: string;
        name: string;
        desc: string;
        ver: string;
        author: string;
        url: string;
        status: "active" | "disable" | "incompatible"
    }
}