/* USER CODE BEGIN Header */

	// WISIELEC

/* USER CODE END Header */
/* Includes ------------------------------------------------------------------*/
#include "main.h"

/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */

#include <string.h>
#include <stdlib.h>
#include <stdio.h>
#include <ctype.h>

/* USER CODE END Includes */

/* Private typedef -----------------------------------------------------------*/
/* USER CODE BEGIN PTD */

/* USER CODE END PTD */

/* Private define ------------------------------------------------------------*/
/* USER CODE BEGIN PD */

#define MAX_TRIES 6
#define GAME_TIME 300   // 5 min - 300 sec

volatile uint32_t remainingTime = GAME_TIME;
volatile uint8_t secondTick = 0;
uint8_t rxChar;

char usedLetters[26];
uint8_t usedCount = 0;

char secretWord[20];
char guessedWord[20];
uint8_t remainingTries = MAX_TRIES;

const char *names[] = {"ALEKS", "LIZA", "JAN", "ANIA"};
const char *animals[] = {"KOT", "PIES", "LEW", "KROLIK"};
const char *countries[] = {"POLSKA", "NIEMCY", "FRANCJA", "HISZPANIA"};

/* USER CODE END PD */

/* Private macro -------------------------------------------------------------*/
/* USER CODE BEGIN PM */

/* USER CODE END PM */

/* Private variables ---------------------------------------------------------*/
TIM_HandleTypeDef htim2;

UART_HandleTypeDef huart2;

/* USER CODE BEGIN PV */

/* USER CODE END PV */

/* Private function prototypes -----------------------------------------------*/
void SystemClock_Config(void);
static void MX_GPIO_Init(void);
static void MX_USART2_UART_Init(void);
static void MX_TIM2_Init(void);
/* USER CODE BEGIN PFP */

/* USER CODE END PFP */

/* Private user code ---------------------------------------------------------*/
/* USER CODE BEGIN 0 */
void UART_Send(char* txt)
{
    HAL_UART_Transmit(&huart2, (uint8_t*)txt, strlen(txt), HAL_MAX_DELAY);
}

char UART_GetChar(void)
{
    char c;
    while (1)
    {
        HAL_UART_Receive(&huart2, (uint8_t*)&c, 1, HAL_MAX_DELAY);

        // ignoruj znaki końca linii
        if (c == '\r' || c == '\n')
            continue;

        return c;
    }
}

void ClearScreen(void)
{
    UART_Send("\033[2J\033[H");
}

void DrawHangman(uint8_t errors)
{
    UART_Send("_______\r\n");
    UART_Send("|/    |\r\n");

    UART_Send("|     ");
    UART_Send(errors > 0 ? "O\r\n" : "\r\n");

    UART_Send("|    ");
    UART_Send(errors > 2 ? "/" : " ");
    UART_Send(errors > 1 ? "|" : " ");
    UART_Send(errors > 3 ? "\\" : " ");
    UART_Send("\r\n");

    UART_Send("|    ");
    UART_Send(errors > 4 ? "/" : " ");
    UART_Send(errors > 5 ? " \\" : " ");
    UART_Send("\r\n");

    UART_Send("|_______\r\n\r\n");
}

void ShowStartScreen(void)
{
    ClearScreen();
    UART_Send(
        "_______\r\n"
        "|/    |\r\n"
        "|     O    GRA W WISIELCA\r\n"
        "|    /|\\   Tworcy: Aleks, Liza\r\n"
        "|    / \\   Milej rozgrywki :)\r\n"
        "|_______\r\n\r\n"

        "|  Wybierz kategorie hasla:\r\n"
        "|  1. Imiona\r\n"
        "|  2. Zwierzeta\r\n"
        "|  3. Kraje\r\n"
    );
}

void InitGame(const char** pool, uint8_t size)
{
    srand(HAL_GetTick());
    strcpy(secretWord, pool[rand() % size]);

    for (uint8_t i = 0; i < strlen(secretWord); i++)
        guessedWord[i] = '_';
    guessedWord[strlen(secretWord)] = 0;

    remainingTries = MAX_TRIES;
    remainingTime = GAME_TIME;
    usedCount = 0;
}

uint8_t LetterUsed(char c)
{
    for (uint8_t i = 0; i < usedCount; i++)
        if (usedLetters[i] == c) return 1;
    return 0;
}

void AddUsedLetter(char c)
{
    usedLetters[usedCount++] = c;
}

uint8_t CheckWin(void)
{
    return strcmp(secretWord, guessedWord) == 0;
}

void ShowGameScreen(void)
{
    char buf[64];
    ClearScreen();
    DrawHangman(MAX_TRIES - remainingTries);

    UART_Send("Haslo: ");
    for (uint8_t i = 0; i < strlen(guessedWord); i++)
    {
        char tmp[2] = { guessedWord[i], '\0' };
        UART_Send(tmp);

        UART_Send(" ");
    }

    sprintf(buf, "\r\n\r\nPozostale proby: %d\r\n", remainingTries);
    UART_Send(buf);

    sprintf(buf, "Pozostaly czas: %02lu:%02lu min\r\n\r\n",
                remainingTime / 60, remainingTime % 60);
        UART_Send(buf);

    UART_Send("Wyprobowane litery: ");
    for (uint8_t i = 0; i < usedCount; i++)
    {
        char tmp[2] = { usedLetters[i], '\0' };
        UART_Send(tmp);

        UART_Send(" ");
    }
    UART_Send("\r\n");
}


// PRZERWANIE - RESET GRY

void HAL_TIM_PeriodElapsedCallback(TIM_HandleTypeDef* htim)
{
    if (htim->Instance == TIM2)
    {
        if (remainingTime > 0)
            remainingTime--;
        secondTick = 1;
    }
}
/* USER CODE END 0 */

/**
  * @brief  The application entry point.
  * @retval int
  */
int main(void)
{
  /* USER CODE BEGIN 1 */
  /* USER CODE END 1 */

  /* MCU Configuration--------------------------------------------------------*/

  /* Reset of all peripherals, Initializes the Flash interface and the Systick. */
  HAL_Init();

  /* USER CODE BEGIN Init */
  /* USER CODE END Init */

  /* Configure the system clock */
  SystemClock_Config();

  /* USER CODE BEGIN SysInit */
  /* USER CODE END SysInit */

  /* Initialize all configured peripherals */
  MX_GPIO_Init();
  MX_USART2_UART_Init();
  MX_TIM2_Init();
  /* USER CODE BEGIN 2 */

  HAL_TIM_Base_Start_IT(&htim2);
  uint8_t gameActive = 1;

  /* USER CODE END 2 */

  /* Infinite loop */
  /* USER CODE BEGIN WHILE */
  while (1)
  {
      gameActive = 0;

      ClearScreen();
      ShowStartScreen();

      // wybór kategorii
      while (1)
      {
          rxChar = UART_GetChar();

          if (rxChar == '1')
          {
              InitGame(names, 4);
              gameActive = 1;
              break;
          }
          else if (rxChar == '2')
          {
              InitGame(animals, 4);
              gameActive = 1;
              break;
          }
          else if (rxChar == '3')
          {
              InitGame(countries, 4);
              gameActive = 1;
              break;
          }
          else if (rxChar == 0x0E) // ctrl+n
          {
              ClearScreen();
              ShowStartScreen();
              continue;
          }
          else
          {
              ClearScreen();
              UART_Send("|\r\n");
              UART_Send("|  Wybierz poprawna opcje!\r\n");
              UART_Send("|\r\n");
              HAL_Delay(1500);
              ShowStartScreen();
          }
      }

      // PĘTLA GRY
      while (gameActive)
      {
          // 1 odswiezanie ekranu co sekundę
          if (secondTick)
          {
              secondTick = 0;
              ShowGameScreen();

              if (remainingTime == 0)
              {
        	  	  HAL_Delay(1000);
                  ClearScreen();
                  UART_Send("|\r\n");
                  UART_Send("|  PRZEGRANA – TIMEOUT!\r\n");
                  UART_Send("|\r\n");
                  HAL_Delay(3000);
                  gameActive = 0;
                  break;
              }
          }

          // 2 odbior znaku UART (nieblokujący krotki timeout)
          if (HAL_UART_Receive(&huart2, (uint8_t *)&rxChar, 1, 10) == HAL_OK)
          {
              // ctrl+n - reset gry
              if (rxChar == 0x0E)
              {
                  ClearScreen();
                  UART_Send("|\r\n");
                  UART_Send("|  RESET GRY!\r\n");
                  UART_Send("|\r\n");
                  HAL_Delay(1500);
                  gameActive = 0;
                  break;
              }

              if (!isalpha(rxChar))
              {
            	  ClearScreen();
            	  UART_Send("|\r\n");
                  UART_Send("|  Uzywaj tylko liter!\r\n");
                  UART_Send("|  Lub znaku resetu gry ctrl+n\r\n");
                  UART_Send("|\r\n");
                  HAL_Delay(1500);
                  continue;
              }

              rxChar = toupper(rxChar);

              if (LetterUsed(rxChar))
              {
            	  ClearScreen();
            	  UART_Send("|\r\n");
                  UART_Send("|  Ta litera juz byla uzyta!\r\n");
                  UART_Send("|\r\n");
                  HAL_Delay(1500);
                  continue;
              }

              AddUsedLetter(rxChar);

              uint8_t hit = 0;
              uint8_t len = strlen(secretWord);
              for (uint8_t i = 0; i < len; i++)
              {
                  if (secretWord[i] == rxChar)
                  {
                      guessedWord[i] = rxChar;
                      hit = 1;
                  }
              }
              if (!hit)
                  remainingTries--;

              ShowGameScreen();

              if (CheckWin())
              {
            	  HAL_Delay(1000);
                  ClearScreen();
                  UART_Send("|\r\n");
                  UART_Send("|  WYGRANA!!!\r\n");
                  UART_Send("|  Poprawnie odgadnieto haslo\r\n");
                  UART_Send("|\r\n");
                  HAL_Delay(3000);
                  gameActive = 0;
                  break;
              }
              else if (remainingTries == 0)
              {
            	  HAL_Delay(1000);
                  ClearScreen();
                  UART_Send("|\r\n");
                  UART_Send("|  PRZEGRANA!\r\n");
                  UART_Send("|  Nie odgadnieto hasla: ");
                  UART_Send("|  ");
                  UART_Send(secretWord);
                  UART_Send("\r\n|\r\n");
                  HAL_Delay(3000);
                  gameActive = 0;
                  break;
              }
          }
      }

    /* USER CODE END WHILE */

    /* USER CODE BEGIN 3 */
  }
  /* USER CODE END 3 */
}

/**
  * @brief System Clock Configuration
  * @retval None
  */
void SystemClock_Config(void)
{
  RCC_OscInitTypeDef RCC_OscInitStruct = {0};
  RCC_ClkInitTypeDef RCC_ClkInitStruct = {0};
  RCC_PeriphCLKInitTypeDef PeriphClkInit = {0};

  /** Initializes the RCC Oscillators according to the specified parameters
  * in the RCC_OscInitTypeDef structure.
  */
  RCC_OscInitStruct.OscillatorType = RCC_OSCILLATORTYPE_HSI;
  RCC_OscInitStruct.HSIState = RCC_HSI_ON;
  RCC_OscInitStruct.HSICalibrationValue = RCC_HSICALIBRATION_DEFAULT;
  RCC_OscInitStruct.PLL.PLLState = RCC_PLL_ON;
  RCC_OscInitStruct.PLL.PLLSource = RCC_PLLSOURCE_HSI;
  RCC_OscInitStruct.PLL.PLLM = 1;
  RCC_OscInitStruct.PLL.PLLN = 10;
  RCC_OscInitStruct.PLL.PLLP = RCC_PLLP_DIV7;
  RCC_OscInitStruct.PLL.PLLQ = RCC_PLLQ_DIV2;
  RCC_OscInitStruct.PLL.PLLR = RCC_PLLR_DIV2;
  if (HAL_RCC_OscConfig(&RCC_OscInitStruct) != HAL_OK)
  {
    Error_Handler();
  }
  /** Initializes the CPU, AHB and APB buses clocks
  */
  RCC_ClkInitStruct.ClockType = RCC_CLOCKTYPE_HCLK|RCC_CLOCKTYPE_SYSCLK
                              |RCC_CLOCKTYPE_PCLK1|RCC_CLOCKTYPE_PCLK2;
  RCC_ClkInitStruct.SYSCLKSource = RCC_SYSCLKSOURCE_PLLCLK;
  RCC_ClkInitStruct.AHBCLKDivider = RCC_SYSCLK_DIV1;
  RCC_ClkInitStruct.APB1CLKDivider = RCC_HCLK_DIV1;
  RCC_ClkInitStruct.APB2CLKDivider = RCC_HCLK_DIV1;

  if (HAL_RCC_ClockConfig(&RCC_ClkInitStruct, FLASH_LATENCY_4) != HAL_OK)
  {
    Error_Handler();
  }
  PeriphClkInit.PeriphClockSelection = RCC_PERIPHCLK_USART2;
  PeriphClkInit.Usart2ClockSelection = RCC_USART2CLKSOURCE_PCLK1;
  if (HAL_RCCEx_PeriphCLKConfig(&PeriphClkInit) != HAL_OK)
  {
    Error_Handler();
  }
  /** Configure the main internal regulator output voltage
  */
  if (HAL_PWREx_ControlVoltageScaling(PWR_REGULATOR_VOLTAGE_SCALE1) != HAL_OK)
  {
    Error_Handler();
  }
}

/**
  * @brief TIM2 Initialization Function
  * @param None
  * @retval None
  */
static void MX_TIM2_Init(void)
{

  /* USER CODE BEGIN TIM2_Init 0 */

  /* USER CODE END TIM2_Init 0 */

  TIM_ClockConfigTypeDef sClockSourceConfig = {0};
  TIM_MasterConfigTypeDef sMasterConfig = {0};

  /* USER CODE BEGIN TIM2_Init 1 */

  /* USER CODE END TIM2_Init 1 */
  htim2.Instance = TIM2;
  htim2.Init.Prescaler = 7999;
  htim2.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim2.Init.Period = 9999;
  htim2.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  htim2.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;
  if (HAL_TIM_Base_Init(&htim2) != HAL_OK)
  {
    Error_Handler();
  }
  sClockSourceConfig.ClockSource = TIM_CLOCKSOURCE_INTERNAL;
  if (HAL_TIM_ConfigClockSource(&htim2, &sClockSourceConfig) != HAL_OK)
  {
    Error_Handler();
  }
  sMasterConfig.MasterOutputTrigger = TIM_TRGO_RESET;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  if (HAL_TIMEx_MasterConfigSynchronization(&htim2, &sMasterConfig) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN TIM2_Init 2 */

  /* USER CODE END TIM2_Init 2 */

}

/**
  * @brief USART2 Initialization Function
  * @param None
  * @retval None
  */
static void MX_USART2_UART_Init(void)
{

  /* USER CODE BEGIN USART2_Init 0 */

  /* USER CODE END USART2_Init 0 */

  /* USER CODE BEGIN USART2_Init 1 */

  /* USER CODE END USART2_Init 1 */
  huart2.Instance = USART2;
  huart2.Init.BaudRate = 115200;
  huart2.Init.WordLength = UART_WORDLENGTH_8B;
  huart2.Init.StopBits = UART_STOPBITS_1;
  huart2.Init.Parity = UART_PARITY_NONE;
  huart2.Init.Mode = UART_MODE_TX_RX;
  huart2.Init.HwFlowCtl = UART_HWCONTROL_NONE;
  huart2.Init.OverSampling = UART_OVERSAMPLING_16;
  huart2.Init.OneBitSampling = UART_ONE_BIT_SAMPLE_DISABLE;
  huart2.AdvancedInit.AdvFeatureInit = UART_ADVFEATURE_NO_INIT;
  if (HAL_UART_Init(&huart2) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN USART2_Init 2 */

  /* USER CODE END USART2_Init 2 */

}

/**
  * @brief GPIO Initialization Function
  * @param None
  * @retval None
  */
static void MX_GPIO_Init(void)
{
  GPIO_InitTypeDef GPIO_InitStruct = {0};

  /* GPIO Ports Clock Enable */
  __HAL_RCC_GPIOC_CLK_ENABLE();
  __HAL_RCC_GPIOH_CLK_ENABLE();
  __HAL_RCC_GPIOA_CLK_ENABLE();
  __HAL_RCC_GPIOB_CLK_ENABLE();

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(LD2_GPIO_Port, LD2_Pin, GPIO_PIN_RESET);

  /*Configure GPIO pin : B1_Pin */
  GPIO_InitStruct.Pin = B1_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_IT_FALLING;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  HAL_GPIO_Init(B1_GPIO_Port, &GPIO_InitStruct);

  /*Configure GPIO pin : LD2_Pin */
  GPIO_InitStruct.Pin = LD2_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
  HAL_GPIO_Init(LD2_GPIO_Port, &GPIO_InitStruct);

}

/* USER CODE BEGIN 4 */

/* USER CODE END 4 */

/**
  * @brief  This function is executed in case of error occurrence.
  * @retval None
  */
void Error_Handler(void)
{
  /* USER CODE BEGIN Error_Handler_Debug */
  /* User can add his own implementation to report the HAL error return state */
  __disable_irq();
  while (1)
  {
  }
  /* USER CODE END Error_Handler_Debug */
}

#ifdef  USE_FULL_ASSERT
/**
  * @brief  Reports the name of the source file and the source line number
  *         where the assert_param error has occurred.
  * @param  file: pointer to the source file name
  * @param  line: assert_param error line source number
  * @retval None
  */
void assert_failed(uint8_t *file, uint32_t line)
{
  /* USER CODE BEGIN 6 */
  /* User can add his own implementation to report the file name and line number,
     ex: printf("Wrong parameters value: file %s on line %d\r\n", file, line) */
  /* USER CODE END 6 */
}
#endif /* USE_FULL_ASSERT */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
