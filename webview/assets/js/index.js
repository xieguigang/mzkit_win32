(() => {

    function temp(res) {
        const { success, result } = res;
        if (success && result) {
            const records = result.records;

            let liTemp = '';
            records.forEach(item => {
                let liItem = `<li>
                <img class="news-pic" src="${item.imgUrl}" />
                <div class="news-txt">
                    <a href="http://v2.biodeep.cn/class/detail?id=${item.id}&page=class">${item.title}</a>
                    <span>${item.createTime}</span>
                </div>
                </li>`;

                liTemp += liItem;
            })

            $('#newsList').html(liTemp)
        }
    }

    $.getJSON('http://v2.biodeep.cn/api/nmdx-cloud-basic/km-curriculum-info/cloud/list?pageNo=1&pageSize=12&sort=new', temp);


})();