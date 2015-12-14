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

//main controller
app.controller('SpellTabCtrl', function($scope, $ionicModal) {

  $scope.spells = [{'name': "Hello Kitty", 'damage': 3, 'imgSrc':"img/rogue_portrait.jpg", 'targetingPattern':'single'},
              {'name': "Dunkey Roll", 'damage': 'millions', 'imgSrc':'img/bullsh.jpg', 'targetingPattern':'angle'},
              {'name': "I'm my own Grandpa", 'damage': 24, 'imgSrc':'img/rogue_portrait.jpg', 'targetingPattern':'mapPoint'},
              {'name': "Benson Turnover", 'damage': 38, 'imgSrc':'img/rogue_portrait.jpg', 'targetingPattern':'line'},
              {'name': "Home Alone 6", 'damage': 40, 'imgSrc':'img/rogue_portrait.jpg', 'targetingPattern':'multi'}
            ];
  $scope.availableTargets = [
    {'name':'Jason Statham', 'hp':'trillions', 'imgSrc':'img/jason.jpg', 'selected':false, 'allocated':0},
    {'name':'Billford Billiams', 'hp':10, 'imgSrc':'img/batman.jpg', 'selected':false, 'allocated':0},
    {'name':'Sparky the WonderDog', 'hp':79, 'imgSrc':'img/sparky.jpg', 'selected':false, 'allocated':0},
  ];

  $scope.selected = {
    'name':'Billford Billiams'
  };


  $scope.possibleDamages = 2; //the number of targets this spell can target

  $scope.addDamage = function(targetName) {
    for (var i = 0; i < $scope.availableTargets.length; i++) {
      if ($scope.availableTargets[i].name == targetName && $scope.possibleDamages > 0)
      {
        $scope.availableTargets[i].allocated = $scope.availableTargets[i].allocated + 1;
        $scope.possibleDamages--;
      }
      if ($scope.availableTargets[i].allocated > 0)
      {
        $scope.availableTargets[i].selected = true;
      }
      if ($scope.availableTargets[i].allocated <= 0)
      {
        $scope.availableTargets[i].selected = false;
      }

    }
  }
  $scope.loseDamage = function(targetName) {

    for (var i = 0; i < $scope.availableTargets.length; i++) {
      if ($scope.availableTargets[i].name == targetName && $scope.availableTargets[i].allocated > 0)
      {
        $scope.availableTargets[i].allocated = $scope.availableTargets[i].allocated - 1;
        $scope.possibleDamages++;
      }
      if ($scope.availableTargets[i].allocated > 0)
      {
        $scope.availableTargets[i].selected = true;
      }
      if ($scope.availableTargets[i].allocated <= 0)
      {
        $scope.availableTargets[i].selected = false;
      }

    }
  }

  $scope.toggleMultiSelected = function(targetName) {
    for (var i = 0; i < $scope.availableTargets.length; i++) {
      if ($scope.availableTargets[i].name == targetName)
      {
        if ($scope.availableTargets[i].selected == true && $scope.availableTargets[i].allocated == 1) {
          $scope.availableTargets[i].selected = false;
          $scope.possibleDamages++;
          $scope.availableTargets[i].allocated = 0;

        } else if ( $scope.availableTargets[i].selected == false && $scope.possibleDamages < 1 ) {
          jQuery(".list").effect("shake", {times:4}, 20);  //shake the list if they try to select more

        } else if ($scope.availableTargets[i].selected == false){  //if the target is not currently selected
          $scope.availableTargets[i].selected = true;
          $scope.possibleDamages--;
          $scope.availableTargets[i].allocated = $scope.availableTargets[i].allocated + 1;
        }
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

        jQuery(".angleTargeting").knob({
          'change' : function(newValue) {
            console.log(newValue);
          }
        });
        jQuery('.angleTargeting').trigger(
          'configure',
          {
              "width":200,
              "min":0,
              "max":100,
              "fgColor":"#222222",
              "angleOffset":-125,
              "thickness":".5",
              "angleArc":250,
              "value":50,
              "cursor":30
          }
        );
        jQuery('.angleTargeting').val(50).trigger('change');
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

    //clean up variables
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

app.controller("MapPointCtrl", function($scope, $ionicModal) {

      console.log("map point ctrl runs");
      jQuery( "#selectable" ).selectable({
        selected: function(event, ui) {
            alert("Selected");
        },
        selecting: function(event, ui) {
            alert("Selecting");
        }
      });
});

app.controller("PlayerCtrl", function($scope) {
  $scope.playerHP = 30;
  $scope.playerMP = 35;
  $scope.playerName = "Gandalf";
  $scope.playerClass = "Rogue";

  $scope.ChangeHP = function(newValue) {
    $scope.playerHP = newValue;
    jQuery('.healthMeter').val($scope.playerHP).trigger('change');
  }

  $scope.ChangeMP = function(newValue) {
    $scope.playerMP = newValue;
    jQuery('.manaMeter').val($scope.playerMP).trigger('change');
  }


  jQuery(".meter").knob({
    'change' : function(v) {console.log(v);},
    'readOnly' : true
  });

  jQuery('.healthMeter').trigger(
    'configure',
    {
        "min":0,
        "max":100,
        "fgColor":"#66CC66",
        "angleOffset":-125,
        "angleArc":250,
        "displayInput":true,
        "readOnly":true
    }
  );
  jQuery('.healthMeter').val($scope.playerHP).trigger('change');

  jQuery('.manaMeter').trigger(
    'configure',
    {
        "min":0,
        "max":100,
        "fgColor":"#0000C4",
        "angleOffset":-125,
        "angleArc":250,
        "displayInput":true,
        "readOnly":true
    }
  );
  jQuery('.manaMeter').val($scope.playerMP).trigger('change');


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
