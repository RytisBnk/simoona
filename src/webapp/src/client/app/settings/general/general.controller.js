(function() {
    'use strict';

    angular
        .module('simoonaApp.Settings')
        .controller('settingsGeneralController', settingsGeneralController);

    settingsGeneralController.$inject = [
        'lodash',
        'localeSrv',
        'settingsRepository',
        'appConfig',
        'errorHandler',
        'notifySrv',
        'Analytics'
    ];

    function settingsGeneralController(lodash, localeSrv, settingsRepository, appConfig, errorHandler, 
        notifySrv, Analytics) {
        /*jshint validthis: true */
        var vm = this;

        vm.isLoading = true;

        vm.languages = null;
        vm.timeZones = null;
        vm.generalSettings = {};
        vm.appConfig = appConfig;
        vm.themes = null;

        vm.saveGeneral = saveGeneral;

        init();

        /////////

        function init() {
            settingsRepository.getGeneralSettings().then(function(response) {
                vm.languages = response.languages;
                vm.timeZones = response.timeZones;
                vm.themes = getThemeSettings();
                
                var alreadySelectedLang = _.find(response.languages, function(lang) { return lang.isSelected });
                vm.generalSettings.languageCode = alreadySelectedLang.name

                var alreadySelectedTz = _.find(response.timeZones, function(tz) { return tz.isSelected });
                vm.generalSettings.timeZoneId = alreadySelectedTz.id;

                vm.generalSettings.themeId = getSelectedTheme();

                vm.isLoading = false;
            }, errorHandler.handleErrorMessage);
        }

        function saveGeneral() {
            settingsRepository.saveGeneralSettings(vm.generalSettings).then(function() {
                vm.isAlreadySubmitted = true;
                Analytics.trackEvent('Settings general', 'save', vm.generalSettings);

                saveThemeSelection(vm.generalSettings.themeId);
                localeSrv.setLocale(vm.generalSettings.languageCode);
                notifySrv.success('common.infoSaved');
            }, errorHandler.handleErrorMessage);
        }

        function getThemeSettings() {
            return [
                {
                    id: "theme-default",
                    displayName: "Light"
                },
                {
                    id : "theme-dark",
                    displayName: "Dark"
                }
            ];
        }

        function getSelectedTheme() {
            return localStorage.getItem("settings.theme.id");
        }

        function saveThemeSelection(selectedThemeId) {
            localStorage.setItem("settings.theme.id", selectedThemeId);
            if (selectedThemeId !== 'theme-dark') {
                document.querySelector('body').classList.remove('theme-dark');
            }
            else {
                document.querySelector('body').classList.add('theme-dark');
            }
        }
    }
}());
