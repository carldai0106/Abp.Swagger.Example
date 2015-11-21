$(function () {

    $("#explore").addClass("btn-primary");

    var inputBase = $("#input_baseUrl");
    var tagName = inputBase.prop("tagName");

    var defaultVal = inputBase.val();

    console.log(swashbuckleConfig.discoveryPaths);

    if (tagName != "SELECT" && swashbuckleConfig.discoveryPaths.length > 1) {
       
        var select = $('<select id="input_baseUrl" name="baseUrl"></select>');

        select
                   .css('margin', '0')
                   .css('border', '1px solid gray')
                   .css('padding', '3px')
                   .css('width', '400px')
                   .css('font-size', '0.9em');

        var rootUrl = swashbuckleConfig.rootUrl;
        $.each(swashbuckleConfig.discoveryPaths, function (index, path) {
            var url = rootUrl + "/" + path;
            var text = path.replace("apidoc/", "").replace(".js", "");
            var option = $('<option value=' + url + '>' + text + '</option>');
            select.append(option);
        });

        select.val(defaultVal);
        inputBase.replaceWith(select);
    }
});