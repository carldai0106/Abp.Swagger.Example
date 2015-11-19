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
      "allOf": [
        {
          "typeName": "NullableIdInputInt32",
          "type": "object",
          "$ref": "#/definitions/NullableIdInputInt32"
        }
      ]
    },
    "NullableIdInputInt32": {
      "typeName": "NullableIdInputInt32",
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
        }
      },
      "allOf": [
        {
          "typeName": "EntityDtoInt32",
          "type": "object",
          "$ref": "#/definitions/EntityDtoInt32"
        }
      ]
    },
    "EntityDtoInt32": {
      "typeName": "EntityDtoInt32",
      "additionalProperties": false,
      "type": "object",
      "required": [
        "Id"
      ],
      "properties": {
        "Id": {
          "type": "integer"
        }
      }
    }
  },
  "responses": {}
}