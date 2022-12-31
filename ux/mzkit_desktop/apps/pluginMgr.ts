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

    const template: string = `
    <tr class="inactive">
    <th scope="row" class="check-column">
        <input type="checkbox" name="check_plugins" >
    </th>
    <td class="plugin-title column-primary">
        <strong>{$name}</strong>
        <div class="row-actions visible">
            <span class="activate">
                <a href=""
                    id="activate-akismet" class="edit"
                    aria-label="Activate Akismet Anti-Spam">Activate</a> |
            </span>
            <span class="delete">
                <a href=""
                    id="delete-akismet" class="delete" aria-label="Delete Akismet Anti-Spam">Delete</a>
            </span>
        </div>

    </td>
    <td class="column-description desc">
        <div class="plugin-description">
            <p>
                {$desc}
            </p>
        </div>
        <div class="inactive second plugin-version-author-uri">
            Version {$ver} | By
            <a href="#">{$author}</a> |
            <a href="{$url}"
                class="thickbox open-plugin-details-modal"
                aria-label="More information about Akismet Anti-Spam"
                data-title="Akismet Anti-Spam">View details</a>
        </div>
    </td>
    <td class="column-auto-updates">
        {$usage_stat}
    </td>
</tr>
    `;
}