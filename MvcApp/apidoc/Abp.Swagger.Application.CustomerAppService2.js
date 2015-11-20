{
  "tags": [
    {
      "name": "Customer",
      "description": "WebApi for Customer."
    }
  ],
  "securityDefinitions": {},
  "paths": {
    "/api/services/app/Customer/GetCustomers": {
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
        "operationId": "GetCustomers",
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
    }
  },
  "responses": {}
}