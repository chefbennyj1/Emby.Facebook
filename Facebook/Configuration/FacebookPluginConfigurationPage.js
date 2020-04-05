define(["require", "loading", "dialogHelper", "formDialogStyle", "emby-checkbox", "emby-select", "emby-toggle"],
    function(require, loading, dialogHelper) {
        var pluginId = "55A5756C-F9EA-4E96-ABF4-6360607FBDCD";
        //https://developers.facebook.com/tools/explorer

        function getUsersToggleHtml(user) {
            var html = '';
            html += '<div class="inputContainer" style="text-align: left;">';
            html += '<label style = "width: auto;" class="mdl-switch mdl-js-switch">';
            html += '<input id="' + user.Id + '" is="emby-toggle" type="checkbox"  class="enableUserWatchUpdate chkAdvanced noautofocus mdl-switch__input" data-embytoggle="true">';
            html += '<span class="toggleButtonLabel mdl-switch__label">' + user.Name + '</span>';
            html += '<div class="mdl-switch__trackContainer">';
            html += '<div class="mdl-switch__track"></div>';
            html += '<div class="mdl-switch__thumb">';
            html += '<span class="mdl-switch__focus-helper"></span>';
            html += '</div>';
            html += '</div> ';
            html += '</label>';
            html += '</div >';
            return html;
        }

        return function (view) {
            view.addEventListener('viewshow',
                () => {

                    var userList = view.querySelector('#userList');

                    ApiClient.getPluginConfiguration(pluginId).then((config) => {

                        ApiClient.getUsers().then(users => {
                            users.forEach(user => {
                                userList.innerHTML += getUsersToggleHtml(user);
                            });

                            var userToggles = userList.querySelectorAll('.enableUserWatchUpdate');
                            userToggles.forEach(toggle => {

                                if (config.UserPostsOptIn.includes(toggle.id)) toggle.checked = true;

                                toggle.addEventListener('change',
                                    (e) => {
                                        if (e.target.checked) {
                                            ApiClient.getPluginConfiguration(pluginId).then((config) => {
                                                config.UserPostsOptIn.push(e.target.id);
                                                ApiClient.updatePluginConfiguration(pluginId, config).then(
                                                    (result) => {
                                                        Dashboard.processPluginConfigurationUpdateResult(result);
                                                    });
                                            });
                                        } else {
                                            ApiClient.getPluginConfiguration(pluginId).then((config) => {
                                                config.UserPostsOptIn =
                                                    config.UserPostsOptIn.filter((id) => id !== e.target.id);
                                                ApiClient.updatePluginConfiguration(pluginId, config).then(
                                                    (result) => {
                                                        Dashboard.processPluginConfigurationUpdateResult(result);
                                                    });
                                            });
                                        }
                                    });
                            });
                            /*
                            ApiClient.getPluginConfiguration(pluginId).then((config) => {
                                if (config.UserPostsOptIn) {
                                    config.UserPostsOptIn.forEach((id) => {
                                        var userToggles = userList.querySelectorAll('.enableUserWatchUpdate');
                                        userToggles.forEach(toggle => {
                                            if (toggle.id === id) {
                                                toggle.checked = true;
                                            }
                                        });

                                    });
                                }
                            });
                            */
                        });
                    });

                        ApiClient.getPluginConfiguration(pluginId).then((config) => {
                        if (config.accessToken) {
                            view.querySelector('#txtFacebookApiKey').value = config.accessToken;
                        }
                        if (config.enableRecommendationUpdate) {
                            view.querySelector('#enableRecommendationUpdate').checked = config.enableRecommendationUpdate;
                        } 
                        if (config.enableUserWatchUpdate) {
                            view.querySelector('#enableUserWatchUpdate').checked = config.enableUserWatchUpdate;
                             
                            if (view.querySelector('#enableUserWatchUpdate').checked) {
                                if (userList.classList.contains('hide')) {
                                    userList.classList.remove('hide');
                                }
                            } else {
                                if (!userList.classList.contains('hide')) {
                                    userList.classList.add('hide');
                                }
                            }
                        } 
                        

                    });

                    view.querySelector('#enableUserWatchUpdate').addEventListener('change', () => {
                        ApiClient.getPluginConfiguration(pluginId).then((config) => {

                            config.enableUserWatchUpdate = view.querySelector('#enableUserWatchUpdate').checked;

                            ApiClient.updatePluginConfiguration(pluginId, config).then((result) => {
                                Dashboard.processPluginConfigurationUpdateResult(result);
                            });
                              
                            if (view.querySelector('#enableUserWatchUpdate').checked) {
                                if (userList.classList.contains('hide')) {
                                    userList.classList.remove('hide');
                                }
                            } else {
                                if (!userList.classList.contains('hide')) {
                                    userList.classList.add('hide');
                                }
                            }

                        });
                    }); 

                    view.querySelector('#enableRecommendationUpdate').addEventListener('change', () => {
                        ApiClient.getPluginConfiguration(pluginId).then((config) => {
                            config.enableRecommendationUpdate = view.querySelector('#enableRecommendationUpdate').checked;
                            ApiClient.updatePluginConfiguration(pluginId, config).then((result) => {
                                Dashboard.processPluginConfigurationUpdateResult(result);
                            });
                        });
                    });

                    view.querySelector('#saveFacebookData').addEventListener('click', (e) => {
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