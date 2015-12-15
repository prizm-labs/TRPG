app = angular.module('combatApp', [
  'angular-meteor',
  'ui.router',
  'ionic',
'ngCordova']);

function onReady() {
  angular.bootstrap(document, ['combatApp']);
}

if (Meteor.isCordova) {
  angular.element(document).on("deviceready", onReady);
}
else {
  angular.element(document).ready(onReady);
}
