app = angular.module('combatApp', [
  //'angular-meteor',
  //'ngCordova'
  'ui.router',
  'ionic'
  ]);


function onReady() {
  angular.bootstrap(document, ['combatApp']);
}

// if (Meteor.isCordova) {
//   angular.element(document).on("deviceready", onReady);
// }
// else {
//   angular.element(document).ready(onReady);
// }

angular.element(document).ready(onReady);
