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
      "properties": {
        "Filter": {
          "type": "string"
        }
      },
      "allOf": [
        {
          "typeName": "PagedAndSortedInputDto",
          "type": "object",
          "allOf": [
            {
              "typeName": "PagedAndSortedBaseInputDto",
              "type": "object",
              "$ref": "#/definitions/PagedAndSortedBaseInputDto"
            }
          ],
          "$ref": "#/definitions/PagedAndSortedInputDto"
        }
      ]
    },
    "PagedAndSortedInputDto": {
      "typeName": "PagedAndSortedInputDto",
      "additionalProperties": false,
      "type": "object",
      "required": [
        "MaxResultCount",
        "SkipCount",
        "PageIndex"
      ],
      "properties": {
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
        }
      },
      "allOf": [
        {
          "typeName": "PagedAndSortedBaseInputDto",
          "type": "object",
          "$ref": "#/definitions/PagedAndSortedBaseInputDto"
        }
      ]
    },
    "PagedAndSortedBaseInputDto": {
      "typeName": "PagedAndSortedBaseInputDto",
      "additionalProperties": false,
      "type": "object",
      "properties": {
        "FieldName": {
          "type": "string"
        }
      }
    },
    "PagedResultOutputCustomerListDto": {
      "typeName": "PagedResultOutputCustomerListDto",
      "additionalProperties": false,
      "type": "object",
      "allOf": [
        {
          "typeName": "PagedResultDtoCustomerListDto",
          "type": "object",
          "allOf": [
            {
              "typeName": "ListResultDtoCustomerListDto",
              "type": "object",
              "$ref": "#/definitions/ListResultDtoCustomerListDto"
            }
          ],
          "$ref": "#/definitions/PagedResultDtoCustomerListDto"
        }
      ]
    },
    "PagedResultDtoCustomerListDto": {
      "typeName": "PagedResultDtoCustomerListDto",
      "additionalProperties": false,
      "type": "object",
      "required": [
        "TotalCount"
      ],
      "properties": {
        "TotalCount": {
          "type": "integer"
        }
      },
      "allOf": [
        {
          "typeName": "ListResultDtoCustomerListDto",
          "type": "object",
          "$ref": "#/definitions/ListResultDtoCustomerListDto"
        }
      ]
    },
    "ListResultDtoCustomerListDto": {
      "typeName": "ListResultDtoCustomerListDto",
      "additionalProperties": false,
      "type": "object",
      "properties": {
        "Items": {
          "items": {
            "typeName": "CustomerListDto",
            "type": "object",
            "allOf": [
              {
                "typeName": "EntityDtoInt32",
                "type": "object",
                "$ref": "#/definitions/EntityDtoInt32"
              }
            ],
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