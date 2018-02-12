/**
* Name:  Angular Simple Table
*
* Version: 1.0.0
*
* Author: Andrei Pericleanu
*		  andrei_pericleanu@yahoo.com  
* 
*  
*/



angular.module("angular-simple-table",[])
    .directive('simpleTable',['$filter','$compile','filterFilter', function($filter,$compile,filterFilter) {
        'use strict';
        window.TableSettings = function (passedData){
            var data = passedData || null;
            var reverseSort = false;
            var currentPage = 1;
            var filteredData = null;
            var rows = 25;
            var orderField = null;
            var searchFields = {};


            //ReverseSort
            this.setReverseSort = function(param){ reverseSort = param; };
            this.getReverseSort = function(){ return reverseSort; };

            //OrderField
            this.setOrderField = function(param){ orderField = param; };
            this.getOrderField = function(){ return orderField; };

            //Data
            this.setData = function(param){ data = param; };
            this.getData = function(){ return data; };

            //Rows per page
            this.setRows = function(param){ this.setPage(1); rows = parseInt(param,10); };
            this.getRows = function(){ return rows; };

            //Page nr
            this.getPage = function(){return currentPage;};
            this.setPage = function(nr){currentPage = parseInt(nr,10);};

            /**
             * getTotalPagesArray
             *
             * @return {Array} - has length = total pages
             */
            this.getTotalPagesArray = function(){
                if(filteredData){
                    return new Array(Math.ceil(filteredData.length/rows))};
            };

            /**
             *  getTotalPages
             *
             * @returns {integer}
             */
            this.getTotalPages = function(){
                if(filteredData){
                    return Math.ceil(filteredData.length/rows)
                }
            };

            /**
             * getFilteredData
             *
             * @returns {Array}  filtered data by search,rows per page and ordered
             */
            this.getFilteredData = function(){
                //convert to array
                if(!(data instanceof Array) && typeof data === 'object' && data !== null){
                    var keys = Object.keys(data);
                    filteredData = data;
                    filteredData = keys.map(function (key) {
                        return filteredData[key]; });
                }else{
                    filteredData = data;
                }

                //searchBy
                if(Object.keys(searchFields).length){
                    var result = [];
                    for (var property in searchFields) {
                        if (searchFields.hasOwnProperty(property)) {
                            //search for each field
                            if(property.indexOf('.') === -1)
                            {
                                var filtered = simpleSearch(searchFields[property],property,filteredData);

                            }else{
                                var filtered = subArraySearch(searchFields[property],property,filteredData);
                            }
                            filtered.forEach(function(elem,index,arr){
                                result.push(elem);
                            });

                        }
                    };
                    filteredData = remove_duplicates(result);
                }
                //orderBy
                if(typeof orderField == 'string'){
                    filteredData = $filter('orderBy')(filteredData, orderField.toLowerCase(), reverseSort);
                }
                if(filteredData && data ){
                    //filter Current Page and Number of Items Per Page
                    var startIndex = (currentPage - 1) * rows;
                    //avoid non-existent page
                    if(startIndex > filteredData.length){
                        this.setPage(1);
                    }
                    return filteredData.slice(startIndex,startIndex + rows);
                }else{
                    filteredData = data;
                }

            };

            function simpleSearch(text,field,data){
                var searched = [];
                var index = field || '$';
                searched[index] = text;
                var result = $filter('filter')(data, searched, false);
                return result;
            };
            function subArraySearch(text,field,data){
                var properties = field.split(".");
                var searchedField = [];
                searchedField[properties[1]] = text ;
                var result = [];
                data.forEach(function(elem,index,arr){
                    var filtered = $filter('filter')(elem[properties[0]],searchedField, false);
                    if(filtered.length > 0){
                        result.push(filteredData[index])
                    }
                });
                return result;
            };
            function remove_duplicates(objectsArray) {
                var usedObjects = {};

                for (var i=objectsArray.length - 1;i>=0;i--) {
                    var so = JSON.stringify(objectsArray[i]);

                    if (usedObjects[so]) {
                        objectsArray.splice(i, 1);

                    } else {
                        usedObjects[so] = true;
                    }
                }

                return objectsArray;

            }

            /**
             *  search
             *
             *  Searches through the data by one or more fields
             *
             * @param text | string  Search String
             * @param field... | string  Name of the field to search by. From the second argument, every argument is a field to search by
             */
            this.search = function(text,field){
                currentPage = 1;
                orderField = null;
                searchFields = {};
                //search by all fields field
                if(arguments.length<2){
                    var field = '$';
                    searchFields[field] = arguments[0];
                }else{
                    //search by multiple fields
                    for (var i = 1; i < arguments.length; i++) {
                        var field = arguments[i];
                        searchFields[field] = arguments[0];
                    }
                }
            };
            /**
             *
             *    Order By
             *
             * @param field | string The name of the field to order by
             * @param keepReverseState  | boolean  If true - reverse state remains the same.
             */
            this.orderBy = function(field,keepReverseState){
                orderField = field;
                if(!keepReverseState){
                    reverseSort = !reverseSort;
                }
            };
        };

        var link = function(scope, iElement, iAttrs){
            $compile(iElement);
            //assign the table to directive's isolated scope
            var table = document.querySelectorAll('[simple-table]');
            var simpleTable = iAttrs['simpleTable'];
            for(var item in table){
                if(table.hasOwnProperty(item)){
                    var currentItem = table[item];
                    if(iAttrs['simpleTable'] === currentItem.getAttribute("simple-table") && currentItem.tagName === "TABLE"){
                        scope.table = table[item];
                    };
                }
            }
            //add thead
            iElement.prepend(document.createElement("tHead"));

            //watch for data change
            scope.$watch(
                function(){
                    var row = scope.table.children[1].children[0];
                    return row;
                },
                function(newValue,oldValue){
                    //if table rows appear for the first time
                    if(typeof newValue !== 'undefined'){
                        var row = newValue;

                        //select tr from thead
                        var headRow = scope.table.children[0];
                        headRow.innerHTML='';
                        var allTds = row.children;
                        var tr = document.createElement("tr");
                        //add th to thead
                        for(var item in allTds){
                            if(allTds.hasOwnProperty(item)){
                                var th = document.createElement("th");

                                var dataSort = allTds[item].getAttribute("data-sort-by");
                                if(dataSort){
                                    var title = allTds[item].getAttribute("title") || '';
                                    th.innerHTML = '<a href="javascript:void(0)" ng-click="'+simpleTable+'.orderBy(\''+dataSort+'\')" >'+
                                        title + '&nbsp;&nbsp;<span ng-show="' + simpleTable+'.getOrderField() == \''+dataSort+'\'"><span ng-show="!'+simpleTable+'.getReverseSort()">&#9650;</span><span ng-show="'+simpleTable+'.getReverseSort()">&#9660;</span></span></a>' ;
                                }else{
                                    th.innerHTML = allTds[item].getAttribute("title");
                                }

                                tr.appendChild(th);
                            }
                        }
                        //compiled the data to the parent scope
                        var compiled = $compile(tr)(scope.$parent);
                        headRow.appendChild(compiled[0]);
                    }
                });
        };

        return {
            scope:{
                simpleTable: "&"
            },
            restrict: 'EA',
            template: '',
            link: link,
        };
    }]);
