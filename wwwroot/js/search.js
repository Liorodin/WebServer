$(function () {
    $('#search-for').submit(async e => {
        e.preventDefault();
        const q = $('#search').val();
        const response = await fetch('/Comments/Search?query=' + q);
        console.log(response);
        const data = await response.json();
        console.log(data);

        const template = $('#template').html();
        let results = '';
        for (var item in data) {
            let row = template;
            for (var key in data[item]) {
                console.log(key, data[item][key]);
                row = row.replaceAll('{' + key + '}', data[item][key]);
                row = row.replaceAll('%7B' + key + '%7D', data[item][key]);
            }
            results += row;
        }
        console.log(results);
        $('#body-search').html(results);
    });

    //self.onmessage = (event) => {
    //    let x = 0;
    //    let i = 0;

    //    for (i = 0; i < event.data.length; i++) {
    //        x += event.data[i];
    //    }

    //    x /= event.data.length;

    //    self.postMessage(x);
    //    self.close();
    //};
});
