/// <reference path="../d/linq.d.ts" />

namespace apps {

    export const biodeep_classroom: string = 'http://v2.biodeep.cn/api/nmdx-cloud-basic/km-curriculum-info/cloud/list?pageNo=1&pageSize=12&sort=new';
    export const biodeep_viewVideo: string = 'http://v2.biodeep.cn/class/detail?id=%s&page=class';

    export interface video {
        imgUrl: string;
        id: string;
        title: string;
        createTime: string;
    }

    export class home extends Bootstrap {

        public get appName(): string {
            return "home";
        }

        protected init(): void {
            let vm = this;

            app.desktop.mzkit
                .GetNewsFeedJSON()
                .then(async function (json) {
                    let text: string = await json;
                    vm.loadList(text);
                });
        }

        private loadList(json_str: string) {
            try {
                this.showClassRoom(JSON.parse(json_str));
            } catch {
                console.error("invalid json response text:");
                console.error(json_str);
            }
        }

        private showClassRoom(res) {
            const { success, result } = res;
            const newsList = $ts("#newsList");

            if (!(success && result)) {
                return newsList.hide();
            } else {
                // newsList.show();
            }

            for (let item of <video[]>result.records) {
                let liItem: string = `
                    <img class="news-pic" src="${item.imgUrl}" />
                    <div class="news-txt">
                        <a href="${sprintf(biodeep_viewVideo, item.id)}">${item.title}</a>
                        <span>${item.createTime}</span>
                    </div>`;
                let li = $ts("<li>").display(liItem);

                newsList.appendElement(li);
            }
        }
    }
}