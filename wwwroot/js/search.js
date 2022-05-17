$(function () {
    $('form').submit(e => {
        e.preventDefault();
        const q = $('#search').val();

        const response = await fetch('/Comments/Search2?query=' + q);
        const data = await response.json();

        console.log(data);
        $('tbody').html('<tr><td>' + data[0].title + '</td></tr>');
    })

});
