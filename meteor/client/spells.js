var app = angular.module('combatApp', [
  'angular-meteor',
  'ui.router',
  'ionic',
  'ngCordova.plugins.datePicker']);


function onReady() {
  angular.bootstrap(document, ['combatApp'], {strictDi: true});
}

// subscribe to the two collections we use
Meteor.subscribe('Projects');
Meteor.subscribe('Tasks');

if (Meteor.isCordova) {
  angular.element(document).on("deviceready", onReady);
}
else {
  angular.element(document).ready(onReady);
}






app.config(function($stateProvider, $urlRouterProvider) {

  $stateProvider
    .state('tabs', {
      url: "/tab",
      abstract: true,
      templateUrl: "templates/tabs.ng.html"
    })
    .state('tabs.home', {
      url: "/home",
      views: {
        'home-tab': {
          templateUrl: "templates/home.ng.html",
          controller: 'SpellTabCtrl'
        }
      }
    })
    .state('tabs.facts', {
      url: "/facts",
      views: {
        'home-tab': {
          templateUrl: "templates/facts.ng.html"
        }
      }
    })
    .state('tabs.facts2', {
      url: "/facts2",
      views: {
        'home-tab': {
          templateUrl: "templates/facts2.ng.html"
        }
      }
    })
    .state('tabs.about', {
      url: "/about",
      views: {
        'about-tab': {
          templateUrl: "templates/about.ng.html"
        }
      }
    })
    .state('tabs.navstack', {
      url: "/navstack",
      views: {
        'about-tab': {
          templateUrl: "templates/nav-stack.ng.html"
        }
      }
    })
   $urlRouterProvider.otherwise("/tab/home");

});

app.controller('SpellTabCtrl', function($scope, $ionicModal) {
  console.log('SpellTabCtrl');


  $scope.spells = [{'name': "Hello Kitty", 'damage': 3, 'imgSrc':"img/rogue_portrait.jpg", 'targetingPattern':'single'},
              {'name': "Dunkey Roll", 'damage': 'millions', 'imgSrc':'img/bullsh.jpg', 'targetingPattern':'angle'},
              {'name': "I'm my own Grandpa", 'damage': 24, 'imgSrc':'img/rogue_portrait.jpg', 'targetingPattern':'mapPoint'},
              {'name': "Benson Turnover", 'damage': 38, 'imgSrc':'img/rogue_portrait.jpg', 'targetingPattern':'line'},
              {'name': "Home Alone 6", 'damage': 40, 'imgSrc':'img/rogue_portrait.jpg', 'targetingPattern':'multi'}
            ];

  $scope.availableTargets = [
    {'name':'Jason Statham', 'hp':'trillions', 'imgSrc':'img/jason.jpg', 'selected':false},
    {'name':'Bill', 'hp':10, 'imgSrc':'img/batman.jpg', 'selected':false},
    {'name':'Sparky the WonderDog', 'hp':79, 'imgSrc':'img/sparky.jpg', 'selected':false},
  ];

  $scope.selected = {
    'name':'bill'
  };

  $scope.toggleMultiSelected = function(targetName) {
    for (var i = 0; i < $scope.availableTargets.length; i++) {
      if ($scope.availableTargets[i].name == targetName)
      {
        $scope.availableTargets[i].selected = !$scope.availableTargets[i].selected;
      }
    }
    console.log("have toggled: " + targetName);
  };

  $ionicModal.fromTemplateUrl('templates/attackPatterns/mapPoint.ng.html', {
      scope: $scope,
      animation: 'slide-in-up'
    }).then(function(modal) {
      $scope.modal = modal;
    });

  $scope.openModal = function(targetingPattern){

    $ionicModal.fromTemplateUrl('templates/attackPatterns/' + targetingPattern + '.ng.html', {
        scope: $scope,
        animation: 'slide-in-up'
      }).then(function(modal) {
        $scope.modal = modal;
        $scope.modal.show();

        jQuery(".healthMeter").knob();
        jQuery('.healthMeter').trigger(
          'configure',
          {
              "width":200,
              "min":10,
              "max":100,
              "fgColor":"#66CC66",
              "angleOffset":-125,
              "angleArc":250,
              "displayInput":true,
              "value":50
          }
        );
      });


    console.log(targetingPattern);
  }

  $scope.closeModal = function(){
    $scope.modal.hide();
  }

  $scope.filterSpells = function (obj, idx){
    return !((obj._index = idx) % 2); //2 columns of spells
  }

  $scope.LaunchAttack = function() {
    //put the meteor call here
    console.log("launching attack!");

    for (var i = 0; i < $scope.availableTargets.length; i++) {  //clear the selected targets
      console.log($scope.availableTargets[i].name + " is: " + $scope.availableTargets[i].selected);
      $scope.availableTargets[i].selected = false;
    }
    $scope.closeModal();
  }

  $scope.CancelAttack = function() {
    //put the meteor call here
    console.log("canceling Attack");

    for (var i = 0; i < $scope.availableTargets.length; i++) {  //clear the selected targets
      console.log($scope.availableTargets[i].name + " is: " + $scope.availableTargets[i].selected);
      $scope.availableTargets[i].selected = false;
    }
    $scope.closeModal();
  }
});

app.controller('LineAttkCtrl', function($scope, $ionicModal) {
  $scope.classUp = "unselectedDirection";
  $scope.classDown = "unselectedDirection";
  $scope.classLeft = "unselectedDirection";
  $scope.classRight = "unselectedDirection";

  $scope.ChooseAttack = function(chosen) {
    $scope.classUp = "unselectedDirection";
    $scope.classDown = "unselectedDirection";
    $scope.classLeft = "unselectedDirection";
    $scope.classRight = "unselectedDirection";

    switch (chosen) {
      case "chooseUp":
        $scope.classUp == "unselectedDirection" ? $scope.classUp = "selectedDirection" : $scope.classUp = "unselectedDirection";
        break;
      case "chooseDown":
        $scope.classDown == "unselectedDirection" ? $scope.classDown = "selectedDirection" : $scope.classDown = "unselectedDirection";
        break;
      case "chooseLeft":
        $scope.classLeft == "unselectedDirection" ? $scope.classLeft = "selectedDirection" : $scope.classLeft = "unselectedDirection";
        break;
      case "chooseRight":
        $scope.classRight == "unselectedDirection" ? $scope.classRight = "selectedDirection" : $scope.classRight = "unselectedDirection";
        break;
      default:
        console.log("chosen direction not one of {up, down, left, right}");
        break;
    }
  }

  $scope.LaunchAttack = function() {
    //put the meteor call here
    console.log("launching attack, special for LINESSS");
    //throw in the chosen direction here
    $scope.closeModal();
  }
});
