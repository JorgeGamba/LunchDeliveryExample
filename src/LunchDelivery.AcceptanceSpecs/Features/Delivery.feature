Feature: Deliver ordered lunches
	In order to ensure the delivery of all ordered lunches
	As the owner
	I want to deliver the lunches simultaneously by multiple drones

Scenario: Deliver only one lunch
	Given the owner scheduled for the day the deliveries
		| Route   |
		| AAAAIAA |
	When the ordered lunches are delivered
	Then the owner should receive a report with the deliveries
		| Delivery                |
		| (-2, 4) dirección Norte |

Scenario: Deliver multiple lunches in only one trip
	Given the owner scheduled for the day the deliveries
		| Route   |
		| AAAAIAA |
		| DDDAIAD |
		| AAIADAD |
	When the ordered lunches are delivered
	Then the owner should receive a report with the deliveries
		| Delivery                  |
		| (-2, 4) dirección Norte   |
		| (-3, 3) dirección Sur     |
		| (-4, 2) dirección Oriente |

Scenario: Deliver requires multiple trips
	Given the owner scheduled for the day the deliveries
		| Route   |
		| AAAAIAA |
		| DDDAIAD |
		| AAIADAD |
		| AAAAAAA |
		| ADAIADA |
		| DAAAAAA |
	When the ordered lunches are delivered
	Then the owner should receive a report with the deliveries
		| Delivery                  |
		| (-2, 4) dirección Norte   |
		| (-3, 3) dirección Sur     |
		| (-4, 2) dirección Oriente |
		| (0, 7) dirección Norte    |
		| (2, 2) dirección Oriente  |
		| (6, 0) dirección Oriente  |

Scenario: Some deliveries are out of reach
	Given the owner scheduled for the day the deliveries
		| Route                  |
		| AAAAIAA                |
		| AAAAAAAAAAA            |
		| DDDAIAD                |
		| AAIADAD                |
		| IADAIADAIADAIADAIADAIA |
	When the ordered lunches are delivered
	Then the owner should receive a report with the deliveries
		| Delivery                  |
		| (-2, 4) dirección Norte   |
		| (-3, 3) dirección Sur     |
		| (-4, 2) dirección Oriente |