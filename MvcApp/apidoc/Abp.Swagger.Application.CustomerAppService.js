{
  "tags": [
    {
      "name": "Customer",
      "description": "WebApi for Customer."
    }
  ],
  "securityDefinitions": {},
  "paths": {
    "/api/services/app/Customer/GetCustomer": {
      "post": {
        "parameters": [
          {
            "in": "body",
            "name": "input",
            "schema": {
              "type": "object",
              "$ref": "#/definitions/NullableIdInput"
            }
          }
        ],
        "tags": [
          "Customer"
        ],
        "summary": "Get customer by id.",
        "operationId": "GetCustomer",
        "responses": {
          "200": {
            "schema": {
              "type": "object",
              "$ref": "#/definitions/GetCustomerForEditOutput"
            }
          }
        }
      }
    },
    "/api/services/app/Customer/GetCustomerById": {
      "post": {
        "parameters": [
          {
            "in": "query",
            "name": "input",            
            "description": "ID of pet to return", 
            "required": true, 
            "type": "integer", 
            "format": "int32"
          }
        ],
        "tags": [
          "Customer"
        ],
        "summary": "Get customer by id.",
        "operationId": "GetCustomerById",
        "responses": {
          "200": {
            "schema": {
              "type": "object",
              "$ref": "#/definitions/GetCustomerForEditOutput"
            }
          }
        }
      }
    },
    "/api/services/app/Customer/GetCustomers": {
      "get": {
        "parameters": [],
        "tags": [],
        "operationId": "GetCustomers",
        "responses": {
          "200": {
            "schema": {
              "items": {
                "type": "object",
                "$ref": "#/definitions/CustomerListDto"
              },
              "type": "array"
            }
          }
        }
      }
    },
    "/api/services/app/Customer/GetCustomerToList": {
      "post": {
        "parameters": [
          {
            "in": "body",
            "name": "input",
            "schema": {
              "type": "object",
              "$ref": "#/definitions/GetCustomersInput"
            }
          }
        ],
        "tags": [
          "Customer"
        ],
        "summary": "Get customers by input.",
        "operationId": "GetCustomerToList",
        "responses": {
          "200": {
            "schema": {
              "type": "object",
              "$ref": "#/definitions/PagedResultOutputCustomerListDto"
            }
          }
        }
      }
    }
  },
  "swagger": "2.0",
  "info": {
    "title": "",
    "version": ""
  },
  "schemes": [],
  "definitions": {
    "NullableIdInput": {
      "typeName": "NullableIdInput",
      "additionalProperties": false,
      "type": "object",
      "properties": {
        "Id": {
          "type": "integer"
        }
      }
    },
    "GetCustomerForEditOutput": {
      "typeName": "GetCustomerForEditOutput",
      "additionalProperties": false,
      "type": "object",
      "required": [
        "Id",
        "FirstName",
        "LastName"
      ],
      "properties": {
        "Id": {
          "type": "integer"
        },
        "FirstName": {
          "type": "string"
        },
        "LastName": {
          "type": "string"
        },
        "Address": {
          "type": "string"
        },
        "City": {
          "type": "string"
        },
        "State": {
          "type": "string"
        },
        "Telephone": {
          "type": "string"
        },
        "Email": {
          "type": "string"
        }
      }
    },
    "CustomerListDto": {
      "typeName": "CustomerListDto",
      "additionalProperties": false,
      "type": "object",
      "required": [
        "Id"
      ],
      "properties": {
        "FirstName": {
          "type": "string"
        },
        "LastName": {
          "type": "string"
        },
        "Address": {
          "type": "string"
        },
        "City": {
          "type": "string"
        },
        "State": {
          "type": "string"
        },
        "Telephone": {
          "type": "string"
        },
        "Email": {
          "type": "string"
        },
        "Id": {
          "type": "integer"
        }
      }
    },
    "GetCustomersInput": {
      "typeName": "GetCustomersInput",
      "additionalProperties": false,
      "type": "object",
      "required": [
        "MaxResultCount",
        "SkipCount",
        "PageIndex"
      ],
      "properties": {
        "Filter": {
          "type": "string"
        },
        "MaxResultCount": {
          "type": "integer",
          "maximum": 1000.0,
          "minimum": 1.0
        },
        "SkipCount": {
          "type": "integer",
          "maximum": 2147483647.0,
          "minimum": 0.0
        },
        "PageIndex": {
          "type": "integer",
          "maximum": 2147483647.0,
          "minimum": 0.0
        },
        "Sorting": {
          "type": "string"
        },
        "FieldName": {
          "type": "string"
        }
      }
    },
    "PagedResultOutputCustomerListDto": {
      "typeName": "PagedResultOutputCustomerListDto",
      "additionalProperties": false,
      "type": "object",
      "required": [
        "TotalCount"
      ],
      "properties": {
        "TotalCount": {
          "type": "integer"
        },
        "Items": {
          "items": {
            "typeName": "CustomerListDto",
            "type": "object",
            "$ref": "#/definitions/CustomerListDto"
          },
          "type": "array"
        }
      }
    }
  },
  "responses": {}
}