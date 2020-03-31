define(["require", "loading", "dialogHelper", "formDialogStyle", "emby-checkbox", "emby-select", "emby-toggle"],
    function(require, loading, dialogHelper) {
        var pluginId = "55A5756C-F9EA-4E96-ABF4-6360607FBDCD";
        //https://developers.facebook.com/tools/explorer
        return function(view) {
            view.addEventListener('viewshow',
                () => {
                    view.querySelector('#saveFacebookAccessToken').addEventListener('click', (e) => {
                        ApiClient.getPluginConfiguration(pluginId).then((config) => {
                            config.accessToken = view.querySelector('#txtFacebookApiKey').value;
                            ApiClient.updatePluginConfiguration(pluginId, config).then((result) => {
                                Dashboard.processPluginConfigurationUpdateResult(result);
                            });
                        });
                    });
                });
        }
    });