/******************************************************************************
Module:  MainPage.xaml.cs
Notices: Copyright (c) by Jeffrey Richter and Wintellect
******************************************************************************/

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Wintellect.AwaitableEvent;

namespace App {
   public sealed partial class MainPage : Page {
      public MainPage() {
         this.InitializeComponent();
         ClearBoard();
      }

      protected override void OnNavigatedTo(NavigationEventArgs e) {
         var t = new MessageDialog("What do you want to do?") {
            Commands = { 
               new UICommand("Tic-Tac-Toe", c => TicTacToe()), 
               new UICommand("Draw", c => Draw()), }
         }.ShowAsync();
      }

      #region Tic-Tac-Toe
      private async void TicTacToe() {
         Debugger.Break();
         var pointerPressed = new AwaitableEvent<PointerRoutedEventArgs>();
         m_grid.PointerPressed += pointerPressed.Handler;

         while (true) {
            // Initialize: Clear board, Turn = X
            ClearBoard();
            Char turn = 'X';

            // Loop until there is a winner (3 in a row)
            for (Boolean winner = false; !winner; ) {
               // Wait for PointerPressed event
               AwaitableEventArgs<PointerRoutedEventArgs> eventArgs = await pointerPressed.RaisedAsync();

               // If square empty, put symbol in square
               Point position = eventArgs.Args.GetCurrentPoint(m_grid).Position;
               var textblock = m_grid.GetAt(
                  (Int32)(position.Y / (m_grid.ActualHeight / 3)),
                  (Int32)(position.X / (m_grid.ActualWidth / 3)));
               if (textblock.Text[0] != c_emptySquare[0]) continue;
               textblock.Text = turn.ToString();

               // If not 3 in a row
               if (!(winner = Winner(turn))) {
                  // If a tie: Show tie dialog, re-initialize
                  if (Tie()) { await new MessageDialog("Tie game").ShowAsync(); break; }

                  // Switch turn (X -> O or O -> X)
                  turn = (turn == 'X') ? 'O' : 'X';
               } else {
                  // If 3 in a row: Show winner dialog, re-initialize
                  await new MessageDialog(turn + " wins!").ShowAsync();
               }
            }
         }
      }

      private const String c_emptySquare = " ";

      private void ClearBoard() {
         for (Int32 row = 0; row < 3; row++)
            for (Int32 col = 0; col < 3; col++)
               m_grid.GetAt(row, col).Text = c_emptySquare;
      }

      private Boolean Tie() {
         for (Int32 row = 0; row < 3; row++)
            for (Int32 col = 0; col < 3; col++)
               if (m_grid.GetAt(row, col).Text == c_emptySquare) return false;
         return true;
      }

      private Boolean Winner(Char charToTest) {
         Boolean winner = false;
         for (Int32 row = 0; row < 3; row++) {
            winner = charToTest == m_grid.GetAt(row, 0).Text[0] &&
                     charToTest == m_grid.GetAt(row, 1).Text[0] &&
                     charToTest == m_grid.GetAt(row, 2).Text[0];
            if (winner) return true;
         }
         for (Int32 col = 0; col < 3; col++) {
            winner = charToTest == m_grid.GetAt(0, col).Text[0] &&
                     charToTest == m_grid.GetAt(1, col).Text[0] &&
                     charToTest == m_grid.GetAt(2, col).Text[0];
            if (winner) return true;
         }
         winner = charToTest == m_grid.GetAt(0, 0).Text[0] &&
                  charToTest == m_grid.GetAt(1, 1).Text[0] &&
                  charToTest == m_grid.GetAt(2, 2).Text[0];
         if (winner) return true;

         winner = charToTest == m_grid.GetAt(0, 2).Text[0] &&
                  charToTest == m_grid.GetAt(1, 1).Text[0] &&
                  charToTest == m_grid.GetAt(2, 0).Text[0];
         return winner;
      }
      #endregion

      #region Draw
      private Int32 m_brushColorIndex = -1;
      private readonly Brush[] m_brushes = new[] {
         new SolidColorBrush(Colors.Red),
         new SolidColorBrush(Colors.Green),
         new SolidColorBrush(Colors.Blue),
         new SolidColorBrush(Colors.Yellow),
         new SolidColorBrush(Colors.Black),
         new SolidColorBrush(Colors.Orange),
      };

      AwaitableEvent<PointerRoutedEventArgs> pointerMoved = new AwaitableEvent<PointerRoutedEventArgs>();
      AwaitableEvent<PointerRoutedEventArgs> pointerReleased = new AwaitableEvent<PointerRoutedEventArgs>();
      AwaitableEvent<PointerRoutedEventArgs> pointerExited = new AwaitableEvent<PointerRoutedEventArgs>();

      private void Draw() {
         // Remove all the tic-tac-toe controls from the grid
         m_grid.Children.Clear();
         m_grid.ColumnDefinitions.Clear();
         m_grid.RowDefinitions.Clear();

         Debugger.Break();
         // Register standard event handlers for PointerPressed & DoubleTapped
         m_grid.PointerPressed += OnPointerPressed;
         m_grid.DoubleTapped += (s, a) => m_grid.Children.Clear();

         // Register awaitable event handlers for PointerMoved, PointerReleased, & PointerExited
         m_grid.PointerMoved += pointerMoved.Handler;
         m_grid.PointerReleased += pointerReleased.Handler;
         m_grid.PointerExited += pointerExited.Handler;
      }
      
      private async void OnPointerPressed(object sender, PointerRoutedEventArgs e) {
         // Drawing is starting: initialize pointer ID, brush, last point, & task we can await until the end
         UInt32 pointerId = e.Pointer.PointerId;
         var brush = m_brushes[m_brushColorIndex = (m_brushColorIndex + 1) % m_brushes.Length];
         PointerPoint lastPoint = e.GetCurrentPoint(m_grid);

         while (true) {
            // Wait until a PointerMoved, PointerReleased, or PointerExited events occurs
            Task<AwaitableEventArgs<PointerRoutedEventArgs>> task = await Task.WhenAny(
               pointerMoved.RaisedAsync(false),
               pointerReleased.RaisedAsync(true),
               pointerExited.RaisedAsync(true));

            // Break out of the loop if PointerReleased or PointerExited
            if ((Boolean)task.AsyncState) break;

            // Process PointerMoved event here
            PointerRoutedEventArgs args = (await task).Args;
            if (!args.Pointer.IsInContact || args.Pointer.PointerId != pointerId) continue;

            PointerPoint currentPoint = e.GetCurrentPoint(m_grid);
            m_grid.Children.Add(new Line {
               X1 = lastPoint.Position.X, Y1 = lastPoint.Position.Y,
               X2 = currentPoint.Position.X, Y2 = currentPoint.Position.Y,
               Stroke = brush, StrokeThickness = 10.0,
               StrokeStartLineCap = PenLineCap.Round,
               StrokeEndLineCap = PenLineCap.Round
            });
            lastPoint = currentPoint;
         }
      }
      #endregion
   }

   internal static class GridExtensions {
      internal static TextBlock GetAt(this Grid grid, Int32 row, Int32 column) {
         return grid.GetElementAt<Border>(row, column).Child.Cast<Viewbox>().Child.Cast<TextBlock>();
      }
      internal static TElement GetElementAt<TElement>(this Grid grid, Int32 row, Int32 column) where TElement : FrameworkElement {
         var elements = from element in grid.Children.OfType<TElement>()
                        where Grid.GetRow(element) == row && Grid.GetColumn(element) == column
                        select element as TElement;
         return elements.FirstOrDefault();
      }
      internal static T Cast<T>(this object o) { return (T)o; }
   }
}
