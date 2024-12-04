## Overview
This repository contains the solution for the practical case study. It is implemented as a C# console application that fulfills the following requirements:
- **Input Transactions**: Allows users to input transactions with details like date, account, type (deposit/withdrawal), and amount.
- **Define Interest Rates**: Supports defining interest rate rules with effective dates and rates.
- **Print Account Statement**: Displays account statements, calculates pro-rated interest rates.
- **Quit**: Exits the application.

---

## Features
1. **Input Transactions**:
   - Allows the user to input transactions in the format:
     ```
     <Date>|<Account>|<Type>|<Amount>
     ```
     Example:
     ```
     14102024|AC001|D|200.00
     ```
   - Automatically creates an account if it does not exist.
   - Displays transactions for the account, ordered by date.
   - **Validation**:
     - For **withdrawals (W)**, checks if the account balance is sufficient:
       - If the balance is insufficient, the transaction will fail, and the user will see a message:
         ```
         Insufficient balance for withdrawal.
         ```

2. **Define Interest Rate Rules**:
   - Allows the user to define interest rate rules in the format:
     ```
     <Date>|<RuleId>|<Rate>
     ```
     Example:
     ```
     01012024|RULE01|1.95
     ```
   - Displays the list of defined rates, ordered by their effective date.
   - **Validation**:
     - Checks if the `RuleId` is unique:
       - If not, the user is prompted:
         ```
         Rule ID is not unique. Do you want to update the existing rule? (Y/N)
         ```
         - If `Y`, the rule input will be updated to the existing Rule ID.
         - If `N`, the user is asked to reenter the Rule ID.
         - If enter a blank line, the user is able to reenter the interest rate rule.

3. **Print Account Statement**:
   - Prompts for the account and month:
     ```
     <Account>|<MMYY>
     ```
     Example:
     ```
     AC001|0924
     ```
   - Displays the account statement, calculates daily interest, and includes it as a transaction at the end of the month.

4. **Quit**:
   - Closes the application.

---

## Error Handling
1. **Unsuccessful Withdrawals**:
   - If a withdrawal transaction is attempted with an amount greater than the account's current balance:
     - The transaction is rejected, and the user is notified:
       ```
       Insufficient balance for withdrawal.
       ```
     - The account balance remains unchanged.

2. **Duplicate Rule IDs**:
   - When defining an interest rate rule, if the `RuleId` already exists:
     - The user is prompted:
       ```
       Rule ID is not unique. Do you want to update the existing rule? (Y/N)
       ```
       - If the user selects **Y**, the rule for the existing Rule ID is updated.
       - If the user selects **N**, the user is asked to reenter the Rule ID and new rule will be added.

3. **Input Validation**:
   - Ensures that all inputs (e.g., date, type, amount) follow the correct format.
   - Displays user-friendly error messages for invalid inputs, such as:
     ```
     Invalid input. Please follow the format: DDMMYYYY|Account|Type|Amount.
     ```

---

## How to Run
1. **Prerequisites**:
   - .NET SDK installed (check using `dotnet --version`).
   - A C# IDE like Visual Studio or Visual Studio Code. (C# extension has to be installed using Visual Studio Code)

2. **Steps**:
   - Clone this repository:
     ```bash
     git clone https://github.com/joceee01/PracticalTestCode.git
     cd BankingApp
     ```
   - Build the project:
     ```bash
     dotnet build
     ```
   - Run the project:
     ```bash
     dotnet run
     ```
